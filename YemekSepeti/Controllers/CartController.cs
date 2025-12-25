using Microsoft.AspNetCore.Mvc;
using YemekSepeti.Models;
using YemekSepeti.Data;
using System.Text.Json;

namespace YemekSepeti.Controllers
{
    public class CartController : Controller
    {
        private readonly MenuItemRepository _menuRepo;

        public CartController(MenuItemRepository menuRepo)
        {
            _menuRepo = menuRepo;
        }

        // ===================== CART SESSION =====================
        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return cartJson == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson)!;
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart",
                JsonSerializer.Serialize(cart));
        }

        // ===================== VIEW CART =====================
        public IActionResult Index()
        {
            return View(GetCart());
        }

        // ===================== ADD TO CART =====================
        public IActionResult AddToCart(int id)
        {
            var menuItem = _menuRepo.GetById(id);
            if (menuItem == null)
                return RedirectToAction("Index", "Restaurants");

            var cart = GetCart();

            // 🔴 Allow only ONE restaurant per cart
            if (cart.Any() && cart.First().RestaurantID != menuItem.RestaurantID)
            {
                TempData["Error"] = "Sepet yalnızca bir restorandan ürün içerebilir.";
                return RedirectToAction("Index");
            }

            var existingItem = cart.FirstOrDefault(c => c.MenuItemId == id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MenuItemId = menuItem.MenuItemID,
                    RestaurantID = menuItem.RestaurantID, // ✅ IMPORTANT
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = 1
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ===================== REMOVE ITEM =====================
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.MenuItemId == id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ===================== CLEAR CART =====================
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }
    }
}
