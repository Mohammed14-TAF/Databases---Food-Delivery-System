using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using System.Text.Json;
using YemekSepeti.Models;
using System.Data;
using System.Data.SqlClient;

namespace YemekSepeti.Controllers
{
    public class OrderController : Controller
    {
        private readonly string _connectionString;

        // ------------------ CONSTRUCTOR ------------------
        public OrderController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ------------------ GET CART FROM SESSION ------------------
        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        // ------------------ CHECKOUT ------------------
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            return View(cart);
        }

        // ------------------ PLACE ORDER ------------------
        [HttpPost]
        [Authorize]
        public IActionResult PlaceOrder()
{
    var cart = GetCart();
    if (!cart.Any())
        return RedirectToAction("Index", "Cart");

    int? userId = HttpContext.Session.GetInt32("UserID");
    if (userId == null)
        return RedirectToAction("Login", "Home");

    // 🔑 REQUIRED VALUES
    int restaurantId = cart.First().RestaurantID;
    decimal totalPrice = cart.Sum(x => x.Price * x.Quantity);

    int orderId;

    using (SqlConnection conn = new SqlConnection(_connectionString))
    {
        conn.Open();

        // 1️⃣ INSERT ORDER (STORED PROCEDURE)
        using (SqlCommand orderCmd = new SqlCommand("AddOrder", conn))
        {
            orderCmd.CommandType = CommandType.StoredProcedure;

            orderCmd.Parameters.AddWithValue("@UserID", userId.Value);
            orderCmd.Parameters.AddWithValue("@RestaurantID", restaurantId);
            orderCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);

            SqlParameter outputId = new SqlParameter("@OrderID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            orderCmd.Parameters.Add(outputId);
            orderCmd.ExecuteNonQuery();

            orderId = (int)outputId.Value;
        }

        // 2️⃣ INSERT ORDER ITEMS
        foreach (var item in cart)
        {
            using (SqlCommand itemCmd = new SqlCommand(
                @"INSERT INTO OrderItems (OrderID, MenuItemID, Quantity, SubTotal)
                  VALUES (@OrderID, @MenuItemID, @Quantity, @SubTotal)",
                conn))
            {
                itemCmd.Parameters.AddWithValue("@OrderID", orderId);
                itemCmd.Parameters.AddWithValue("@MenuItemID", item.MenuItemId);
                itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                itemCmd.Parameters.AddWithValue("@SubTotal", item.Price * item.Quantity);

                itemCmd.ExecuteNonQuery();
            }
        }
    }

    // 3️⃣ CLEAR CART
    HttpContext.Session.Remove("Cart");

    return RedirectToAction("Success");
}

        // ------------------ ORDER HISTORY ------------------
        [Authorize]
        public IActionResult History()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
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

        // ------------------ SUCCESS ------------------
        public IActionResult Success()
        {
            return View();
        }
    }
}
 