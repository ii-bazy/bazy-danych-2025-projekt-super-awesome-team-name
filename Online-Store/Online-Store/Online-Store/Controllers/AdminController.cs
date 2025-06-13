using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Store.DB;
using Online_Store.Models;
using System.Globalization;

namespace Online_Store.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly Dictionary<int,ViewProduct> _products;
        private readonly Dictionary<int, ViewUser> _users;
        private readonly Dictionary<int, ViewOrderGroups> _orderGroups;
        public AdminController() 
        {
            // TODO: Use Data Layer to get products, users and orders
            _products = new Dictionary<int, ViewProduct>();
            _users = new Dictionary<int, ViewUser>();
            _orderGroups = new Dictionary<int, ViewOrderGroups>();
        }

        public IActionResult Index()
        {
            return View(_products);
        }

        [HttpPost]
        public IActionResult AddProduct(string name, string description, string price)
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

            var newProduct = new ViewProduct
            {
                Name = name,
                Description = description,
                Price = Math.Round(parsedPrice, 2)
            };

            // TODO: Use Data Layer to add new Product and get real id

            Random random = new Random();
            int id = random.Next(1, 1000);
            _products[id] = newProduct;

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
            return View();
        }

        // Edition's form handler
        [HttpPost]
        public IActionResult Edit(KeyValuePair<int, ViewProduct> model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Use id and ViewProduct to update Products table

                _products[model.Key] = model.Value;

                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "There was an error updating the product.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int productId)
        {

            // TODO: Delete from database
            var success = true;
            if (success)
            {
                _products.Remove(productId);
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting product.";
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Users()
        {
            return View(_users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {

            // TODO: Delete from database
            var success = true;
            if (success)
            {
                _users.Remove(userId);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult Orders()
        {
            return View(_orderGroups);
        }

        [HttpPost]
        public IActionResult DeleteOrder(int orderId)
        {

            // TODO: Delete from database
            var success = true;
            if (success)
            {
                _orderGroups.Remove(orderId);
                TempData["SuccessMessage"] = "Order deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting order.";
            }
            return RedirectToAction("Orders");
        }
    }
}
