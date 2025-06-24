using Microsoft.AspNetCore.Mvc;
using Online_Store.Data;
using Online_Store.Extensions;
using Online_Store.Models;
using Online_Store.Services;

namespace Online_Store.Controllers
{
    public class AnonController : Controller
    {
        private readonly IEnumerable<ViewProduct> _products;

        public AnonController(IService service)
        {
            _products = service.GetProducts();
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
