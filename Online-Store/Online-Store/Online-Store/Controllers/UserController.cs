using Microsoft.AspNetCore.Mvc;
using Online_Store.Extensions;
using Online_Store.Models;
using Online_Store.Services;

namespace Online_Store.Controllers
{
    public class UserController : Controller
    {
        private readonly IService _service;
        private readonly Dictionary<int, ViewProduct> _products;

        public UserController(IService service)
        {
            _service = service;
            _products = _service.GetIdViewProducts();
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

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            _service.AddToCart(User.Identity?.Name , productId);

            TempData["SuccessMessage"] = $"{_products[productId].Name} has been added to your cart!";
            return RedirectToAction("Index");
        }
    }
}
