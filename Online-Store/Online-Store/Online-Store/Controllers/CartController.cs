using Microsoft.AspNetCore.Mvc;
using Online_Store.Extensions;
using Online_Store.Models;
using Online_Store.Services;

namespace Online_Store.Controllers
{
    public class CartController : Controller
    {
        private readonly IService _service;

        public CartController(IService service)
        {
            _service = service;
        }
        private Dictionary<int, ViewCartItem> GetCart()
        {
            return _service.GetCartItems(User.Identity?.Name);
        }

        // Display Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Remove from cart
        [HttpPost]
        public IActionResult RemoveFromCart(int key, string name)
        {
            _service.DeleteOrderItem(key);
            
            TempData["SuccessMessage"] = $"{name} has been removed from your cart!";
            
            return RedirectToAction("Index");
        }
    }
}
