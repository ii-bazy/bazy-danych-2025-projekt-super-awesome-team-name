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
        private Dictionary<int, ViewNotification> _notifications;

        public UserController(IService service)
        {
            _service = service;
            _products = _service.GetIdViewProducts();
        }

        // Display all products for user
        public IActionResult Index()
        {
            _notifications = _service.GetIdSendNotifications(User.Identity?.Name);
            bool flag = _notifications.Any(n => !n.Value.IsRead);

            var model = new ViewProductsWithNotificationFlag()
            {
                Pairs = _products,
                HasUnreadNotification = flag
            };

            return View(model);
        }

        // Display only wanted products
        public IActionResult Search(string query)
        {
            _notifications = _service.GetIdSendNotifications(User.Identity?.Name);

            query = query?.ToLower() ?? "";

            var result = _products.Where(
                p => p.Value.Name.ToLower().Contains(query) ||
                p.Value.Description.ToLower().Contains(query)).ToList();
            bool flag = _notifications.Any(n => !n.Value.IsRead);

            var model = new ViewProductsWithNotificationFlag()
            {
                Pairs = result,
                HasUnreadNotification = flag
            };

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            _service.AddToCart(User.Identity?.Name , productId);

            TempData["SuccessMessage"] = $"{_products[productId].Name} has been added to your cart!";
            return RedirectToAction("Index");
        }

        public IActionResult Notifications()
        {
            _notifications = _service.GetIdSendNotifications(User.Identity?.Name);
            return View("Notifications", _notifications);
        }

        [HttpPost]
        public IActionResult UpdateNotification(int notificationId)
        {
            _service.ChangeNotificationIsRead(notificationId);

            TempData["SuccessMessage"] = $"Notification's status is changed";
            return RedirectToAction("Notifications");
        }
    }
}
