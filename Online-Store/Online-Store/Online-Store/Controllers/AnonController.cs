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

    }
}
