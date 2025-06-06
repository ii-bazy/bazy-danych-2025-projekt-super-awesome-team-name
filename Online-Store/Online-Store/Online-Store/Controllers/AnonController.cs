using Microsoft.AspNetCore.Mvc;
using Online_Store.DB;
using Online_Store.Extensions;
using Online_Store.Models;

namespace Online_Store.Controllers
{
    public class AnonController : Controller
    {
        private readonly List<ViewProduct> _products;

        public AnonController()
        {
            // TODO: Use Data Layer to display Orders
            _products = new List<ViewProduct>();
        }

        // Display all products for anon user
        public IActionResult Index()
        {
            return View(_products);
        }

        // Display only wanted products
        public IActionResult Search(string query)
        {
            var result = _products.Where(p => p.Name.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                                           || p.Description.Contains(query, System.StringComparison.OrdinalIgnoreCase)).ToList();
            return View("Index", result);
        }


        [HttpPost]
        public IActionResult AddToCart(string productName, double productPrice)
        {
            // For anonymus users we store cart in session
            var cart = HttpContext.Session.GetObjectFromJson<List<ViewCartItem>>("cart") ?? new List<ViewCartItem>();

            var existingItem = cart.FirstOrDefault(c => c.Name == productName);
            if (existingItem == null)
            {
                cart.Add(new ViewCartItem { Name = productName, Price = productPrice, Quantity = 1 });
            }
            else
            {
                existingItem.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);
            TempData["SuccessMessage"] = $"{productName} has been added to your cart!";
            return RedirectToAction("Index");
        }
    }
}
