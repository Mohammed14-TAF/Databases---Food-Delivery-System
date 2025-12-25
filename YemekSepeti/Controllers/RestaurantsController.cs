using Microsoft.AspNetCore.Mvc;
using YemekSepeti.Data;
using YemekSepeti.Models;

namespace YemekSepeti.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly RestaurantRepository _restaurantRepo;
        private readonly MenuItemRepository _menuRepo;

        public RestaurantsController(RestaurantRepository restaurantRepo, MenuItemRepository menuRepo)
        {
            _restaurantRepo = restaurantRepo;
            _menuRepo = menuRepo;
        }

        // ---------------------------------------------------------
        //  SHOW ALL RESTAURANTS + SEARCH + CATEGORY FILTER
        // ---------------------------------------------------------
        public IActionResult Index(string? search, string? category)
        {
            var restaurants = _restaurantRepo.GetAll();

            // SEARCH FILTER
            if (!string.IsNullOrWhiteSpace(search))
            {
                restaurants = restaurants
                    .Where(r =>
                        r.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        r.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            // CATEGORY FILTER
            if (!string.IsNullOrWhiteSpace(category))
            {
                restaurants = restaurants
                    .Where(r => r.Category != null &&
                                r.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.SelectedCategory = category;
            ViewBag.Search = search;

            return View(restaurants);
        }

        // ---------------------------------------------------------
        //  DEDICATED SEARCH PAGE (HOME SEARCH BAR)
        // ---------------------------------------------------------
        public IActionResult Search(string q, string? location)
        {
            if (string.IsNullOrWhiteSpace(q))
                return RedirectToAction("Index");

            var restaurants = _restaurantRepo.Search(q);
            var menuItems = _menuRepo.Search(q);

            ViewBag.Query = q;
            ViewBag.MenuResults = menuItems;

            return View(restaurants);
        }

        // ---------------------------------------------------------
        //  MENU + CATEGORY FILTER + SEARCH + SORTING
        // ---------------------------------------------------------
        public IActionResult Menu(int id, string? category, string? search, string? sort)
        {
            var restaurant = _restaurantRepo.GetById(id);
            if (restaurant == null)
                return NotFound();

            var items = _menuRepo.GetByRestaurant(id);

            // FILTER BY CATEGORY
            if (!string.IsNullOrWhiteSpace(category))
                items = items.Where(i => i.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

            // SEARCH INSIDE MENU
            if (!string.IsNullOrWhiteSpace(search))
                items = items.Where(i =>
                    i.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    i.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();

            // SORTING
            if (sort == "low")
                items = items.OrderBy(i => i.Price).ToList();
            else if (sort == "high")
                items = items.OrderByDescending(i => i.Price).ToList();

            // SEND EXTRA DATA TO VIEW
            ViewBag.RestaurantName = restaurant.Name;
            ViewBag.RestaurantImage = restaurant.ImageUrl;
            ViewBag.Categories = items.Select(i => i.Category).Distinct().ToList();
            ViewBag.SelectedCategory = category;
            ViewBag.Search = search;
            ViewBag.Sort = sort;

            return View(items);
        }
    }
}
