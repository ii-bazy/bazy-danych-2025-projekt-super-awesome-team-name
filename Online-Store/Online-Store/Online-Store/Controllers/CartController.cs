using Microsoft.AspNetCore.Mvc;
using Online_Store.Extensions;
using Online_Store.Models;

namespace Online_Store.Controllers
{
    public class CartController : Controller
    {
        private List<ViewCartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<ViewCartItem>>("cart");
            return cart ?? new List<ViewCartItem>();
        }

        private void SaveCart(List<ViewCartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson("cart", cart);
        }

        // Display Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Remove from cart
        [HttpPost]
        public IActionResult RemoveFromCart(string productName)
        {
            var cart = GetCart();
            var itemToRemove = cart.FirstOrDefault(c => c.Name == productName);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
                TempData["SuccessMessage"] = $"{productName} has been removed from your cart!";
            }

            return RedirectToAction("Index");
        }
    }
}
