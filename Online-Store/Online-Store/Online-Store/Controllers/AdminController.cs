using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Store.Data.Models;
using Online_Store.Models;
using Online_Store.Services;
using System.Globalization;

namespace Online_Store.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly Dictionary<int,ViewProduct> _products;
        private readonly Dictionary<int, ViewUser> _users;
        private readonly Dictionary<int, ViewOrderGroup> _orderGroups;
        private readonly IService _service;
        public AdminController(IService service) 
        {
            _service = service;
            _products = _service.GetIdViewProducts();
            _users = _service.GetIdViewUsers();
            _orderGroups = _service.GetIdPayedOrderGroups();
        }

        public IActionResult Index()
        {
            return View(_products);
        }

        [HttpPost]
        public IActionResult AddProduct(string name, string description, string price, string quantity)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(price))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToAction("Index");
            }

            if (!double.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedPrice) || parsedPrice <= 0)
            {
                TempData["ErrorMessage"] = "Invalid or negative price. Please enter a valid number.";
                return RedirectToAction("Index");
            }

            if (!int.TryParse(quantity, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedQuantity) || parsedPrice <= 0)
            {
                TempData["ErrorMessage"] = "Invalid or negative quantity. Please enter a valid number.";
                return RedirectToAction("Index");
            }

            var newProduct = new ViewProduct
            {
                Name = name,
                Description = description,
                Price = Math.Round(parsedPrice, 2)
            };

            _service.AddProduct(name, description, (float)Math.Round(parsedPrice, 2), parsedQuantity);

            TempData["SuccessMessage"] = $"{name} has been added!";
            return RedirectToAction("Index");
        }

        // Displays edition form
        public IActionResult Edit(int productId)
        {
            var product = _products[productId];
            if (product == null)
            {
                return NotFound();
            }
            var item = new KeyValuePair<int, ViewProduct>(productId, product);

            return View(item);
        }

        // Edition's form handler
        [HttpPost]
        public IActionResult Edit(int id, string name, string description, float price, int quantity)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateProduct(
                    id, 
                    name, 
                    description,
                    (float) Math.Round(price, 2), 
                    quantity);

                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "There was an error updating the product.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int productId)
        {
            _service.DeleteProduct(productId);

            TempData["SuccessMessage"] = "Product deleted successfully!";
            
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Users()
        {
            return View(_users);
        }

        [HttpPost]
        public IActionResult DeleteUserAsync(int userId)
        {

            _service.DeleteUser(userId);

            TempData["SuccessMessage"] = "User deleted successfully!";

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult Orders()
        {
            return View(_orderGroups);
        }

        [HttpPost]
        public IActionResult DeleteOrder(int orderGroupId)
        {
            _service.DeleteOrderGroup(orderGroupId);

            TempData["ErrorMessage"] = "Error deleting order.";
            
            return RedirectToAction("Orders");
        }
    }
}
