using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Online_Store.Data.Models;
using Online_Store.Extensions;
using Online_Store.Models;
using Online_Store.Services;
using System.Collections.Generic;
using System.Numerics;

namespace Online_Store.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IService _service;
        private Dictionary<int, ViewCartItem> GetCart()
        {
            return _service.GetCartItems(User.Identity?.Name);
        }

        public PaymentController(IService service)
        {
            _service = service;
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

            using (var transaction = _service.BeginTransaction())
            {
                try
                {
                    var result = _service.BuyCart(User.Identity?.Name);
                    if (result.Succes)
                        TempData["PaymentResult"] = "Your order has been placed successfully. The admin can now view your order via the order list.";
                    else
                        TempData["PaymentResult"] = $"At least one of items in your cart ({result.ItemName}) isn't avaible now. You will receive a notification as it appears. For now in order to buy a cart, remove that product.";

                    transaction.Commit();
                    
                    return RedirectToAction("OrderPlaced");
                }
                catch
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Something went wrong while placing the order.";
                    return RedirectToAction("Index");
                }
            }
        }


        [HttpGet]
        public IActionResult OrderPlaced()
        {
            return View();
        }
    }
}
