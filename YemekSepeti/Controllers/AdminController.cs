using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using YemekSepeti.Data;
using YemekSepeti.Models;

namespace YemekSepeti.Controllers
{
    public class AdminController : Controller
    {
        private readonly RestaurantRepository _restaurantRepo;
        private readonly MenuItemRepository _menuItemRepo;
        private readonly string _connectionString;

        // ------------------ CONSTRUCTOR ------------------
        public AdminController(
            RestaurantRepository restaurantRepo,
            MenuItemRepository menuItemRepo,
            IConfiguration configuration)
        {
            _restaurantRepo = restaurantRepo;
            _menuItemRepo = menuItemRepo;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ------------------ ADMIN CHECK ------------------
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // ====================== DASHBOARD ======================
        public IActionResult AdminDashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            return View();
        }

        // ====================== ALL ORDERS (NEW) ======================
        public IActionResult Orders()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT OrderID, UserID, OrderDate, Status, TotalPrice
                      FROM Orders
                      ORDER BY OrderDate DESC",
                    conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderID = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                OrderDate = reader.GetDateTime(2),
                                Status = reader.GetString(3),
                                TotalPrice = reader.IsDBNull(4)
                                    ? 0
                                    : reader.GetDecimal(4),

                            });
                        }
                    }
                }
            }

            return View(orders);
        }

        // ====================== RESTAURANTS ======================
        public IActionResult Restaurants()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            return View(_restaurantRepo.GetAll());
        }

        public IActionResult AddRestaurant()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult AddRestaurant(Restaurant model, IFormFile? ImageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            model.ImageUrl = SaveImage(
                ImageFile,
                "wwwroot/images/restaurants/",
                "/images/restaurants/",
                "/images/default-restaurant.png"
            );

            _restaurantRepo.Add(model);
            return RedirectToAction("Restaurants");
        }

        public IActionResult EditRestaurant(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            return View(_restaurantRepo.GetById(id));
        }

        [HttpPost]
        public IActionResult EditRestaurant(Restaurant model, IFormFile? ImageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (ImageFile != null)
            {
                model.ImageUrl = SaveImage(
                    ImageFile,
                    "wwwroot/images/restaurants/",
                    "/images/restaurants/",
                    model.ImageUrl
                );
            }

            _restaurantRepo.Update(model);
            return RedirectToAction("Restaurants");
        }

        public IActionResult DeleteRestaurant(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            _restaurantRepo.Delete(id);
            return RedirectToAction("Restaurants");
        }

        // ====================== MENU ITEMS ======================
        public IActionResult RestaurantMenu(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            ViewBag.RestaurantID = id;
            ViewBag.RestaurantName = _restaurantRepo.GetById(id)?.Name;

            return View(_menuItemRepo.GetByRestaurant(id));
        }

        public IActionResult AddMenuItem(int restaurantId)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            ViewBag.RestaurantID = restaurantId;
            return View();
        }

        [HttpPost]
        public IActionResult AddMenuItem(MenuItem item, IFormFile? ImageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            item.ImageUrl = SaveImage(
                ImageFile,
                "wwwroot/images/menu/",
                "/images/menu/",
                "/images/no-food.png"
            );

            _menuItemRepo.Add(item);
            return RedirectToAction("RestaurantMenu", new { id = item.RestaurantID });
        }

        public IActionResult EditMenuItem(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            return View(_menuItemRepo.GetById(id));
        }

        [HttpPost]
        public IActionResult EditMenuItem(MenuItem item, IFormFile? ImageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (ImageFile != null)
            {
                item.ImageUrl = SaveImage(
                    ImageFile,
                    "wwwroot/images/menu/",
                    "/images/menu/",
                    item.ImageUrl
                );
            }

            _menuItemRepo.Update(item);
            return RedirectToAction("RestaurantMenu", new { id = item.RestaurantID });
        }

        public IActionResult DeleteMenuItem(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            var item = _menuItemRepo.GetById(id);
            if (item != null)
                _menuItemRepo.Delete(id);

            return RedirectToAction("RestaurantMenu", new { id = item?.RestaurantID });
        }

        // ====================== IMAGE HELPER ======================
        private string SaveImage(
            IFormFile? file,
            string folder,
            string urlPrefix,
            string defaultImage)
        {
            if (file == null || file.Length == 0)
                return defaultImage;

            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return urlPrefix + fileName;
        }
    }
}
