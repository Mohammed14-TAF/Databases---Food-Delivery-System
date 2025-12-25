using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using YemekSepeti.Models;
using Microsoft.Data.SqlClient;
using YemekSepeti.Data;

namespace YemekSepeti.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserRepository _userRepo;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, UserRepository userRepo, IConfiguration configuration)
        {
            _logger = logger;
            _userRepo = userRepo;
            _configuration = configuration;
        }

        // ------------------ MAIN PAGES ------------------

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

 

public IActionResult MyOrder()
{
    int? userId = HttpContext.Session.GetInt32("UserID");
    if (userId == null)
        return RedirectToAction("Login");

    List<Order> orders = new List<Order>();

    using (SqlConnection conn = new SqlConnection(
        _configuration.GetConnectionString("DefaultConnection")))
    {
        conn.Open();

        using (SqlCommand cmd = new SqlCommand(
            @"SELECT OrderID, OrderDate, Status, TotalPrice
              FROM Orders
              WHERE UserID = @UserID
              ORDER BY OrderDate DESC",
            conn))
        {
            cmd.Parameters.AddWithValue("@UserID", userId.Value);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = reader.GetInt32(0),
                        OrderDate = reader.GetDateTime(1),
                        Status = reader.GetString(2),
                        TotalPrice = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3)
                    });
                }
            }
        }
    }

    return View(orders);
}


        public IActionResult Blog()
        {
            return View();
        }

        // ------------------ LOGIN ------------------

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl)
        {
            var user = _userRepo.ValidateUser(email, password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // 🔐 CREATE CLAIMS (AUTHENTICATION COOKIE)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // ✅ SIGN IN (THIS MAKES [Authorize] WORK)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            // ✅ KEEP SESSION (OPTIONAL BUT OK)
            HttpContext.Session.SetInt32("UserID", user.Id);
            HttpContext.Session.SetString("FullName", user.Name);
            HttpContext.Session.SetString("Role", user.Role);

            // 🔁 Redirect back to Checkout if blocked earlier
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            if (user.Role == "Admin")
                return RedirectToAction("AdminDashboard", "Admin");

            return RedirectToAction("Index");
        }

        // ------------------ LOGOUT ------------------

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // ------------------ SIGNUP ------------------

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(string name, string email, string password)
        {
            bool success = _userRepo.CreateUser(name, email, password);

            if (!success)
            {
                ViewBag.Error = "This email is already registered.";
                return View();
            }

            return RedirectToAction("Login");
        }

        // ------------------ ERROR ------------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
