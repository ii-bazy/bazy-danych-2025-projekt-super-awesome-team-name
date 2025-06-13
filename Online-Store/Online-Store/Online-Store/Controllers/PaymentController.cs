using Microsoft.AspNetCore.Mvc;
using Online_Store.DB;
using Online_Store.Extensions;
using Online_Store.Models;

namespace Online_Store.Controllers
{
    public class PaymentController : Controller
    {
        private List<ViewCartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<ViewCartItem>>("cart");
            return cart ?? new List<ViewCartItem>();
        }

        private void ClearCart()
        {
            HttpContext.Session.SetObjectAsJson("cart", new List<ViewCartItem>());
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult PlaceOrder()
        {
            var cart = GetCart();

            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            // TODO: Place Order in database

            ClearCart();

            TempData["SuccessMessage"] = "Order placed successfully! Your cart is now empty.";
            return RedirectToAction("OrderPlaced");
        }



        [HttpGet]
        public IActionResult OrderPlaced()
        {
            return View();
        }
    }
}
