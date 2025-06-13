using Microsoft.AspNetCore.Mvc;
using Online_Store.DB;
using Online_Store.Extensions;
using Online_Store.Models;

namespace Online_Store.Controllers
{
    public class UserController : Controller
    {

        private readonly Dictionary<int, ViewProduct> _products;

        public UserController()
        {
            // TODO: Use Data Layer to display Orders
            _products = new Dictionary<int, ViewProduct>();
        }

        // Display all products for user
        public IActionResult Index()
        {
            return View(_products);
        }

        // Display only wanted products
        public IActionResult Search(string query)
        {
            var result = _products.Where(p => p.Value.Name.Contains(query) || p.Value.Description.Contains(query)).ToList();
            return View("Index", result);
        }


        // TODO: Find better way to store Cart
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<ViewCartItem>>("cart") ?? new List<ViewCartItem>();

            var existingItem = cart.FirstOrDefault(c => c.Name == _products[productId].Name);
            if (existingItem == null)
            {
                cart.Add(new ViewCartItem { Name = _products[productId].Name, Price = _products[productId].Price, Quantity = 1 });
            }
            else
            {
                existingItem.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);
            TempData["SuccessMessage"] = $"{_products[productId].Name} has been added to your cart!";
            return RedirectToAction("Index");
        }
    }
}
