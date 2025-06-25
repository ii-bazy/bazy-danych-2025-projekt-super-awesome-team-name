using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Store.Models;
using System.Security.Claims;
using Online_Store.Services;

namespace Online_Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly IService _service;
        public AccountController(IService service) 
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View(new ViewAccount());
        }

        [HttpPost]
        public IActionResult SignIn(ViewAccount model)
        {
            // TODO: Hash
            var user = _service.GetByUsernameAndPassword(model.Username, model.Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false // Saves even after clowsing brawser
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                         new ClaimsPrincipal(claimsIdentity),
                                         authProperties).Wait(); // Wait for sync

                TempData["SuccessMessage"] = $"Welcome, {user.Username}!";
                return RedirectToAction("Index", "User");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult SignInAdmin()
        {
            return View(new ViewAccount());
        }

        [HttpPost]
        public IActionResult SignInAdmin(ViewAccount model)
        {
            var admin = _service.GetByUsernameAndPassword(model.Username, model.Password);

            if (admin != null)
            {
                var claims = new List<Claim>
            {
                new(ClaimTypes.Name, admin.Username),
                new(ClaimTypes.Role, "Admin")
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true 
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                         new ClaimsPrincipal(claimsIdentity),
                                         authProperties).Wait();

                TempData["SuccessMessage"] = $"Welcome,  Admin {admin.Username}!";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid admin credentials.";
                return View("SignInAdmin", model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Anon");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new ViewAccount());
        }

        [HttpPost]
        public IActionResult SignUp(ViewAccount model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.ErrorMessage = "Username and password cannot be empty.";
                return View(model);
            }

            bool userExists = _service.IsUsernameUsed(model.Username);

            if (userExists)
            {
                ViewBag.ErrorMessage = "This username is already taken.";
                return View(model); 
            }

            _service.AddUser(model.Username, model.Password, "Customer");

            TempData["SuccessMessage"] = "Your account has been created. You can now log in!";
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult SignUpAdmin()
        {
            ViewBag.ErrorMessage = null;
            return View(new ViewAccount());
        }

        [HttpPost]
        public IActionResult SignUpAdmin(ViewAccount model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.ErrorMessage = "Username and password cannot be empty.";
                return View(model);
            }

            bool userExists = _service.IsUsernameUsed(model.Username);

            if (userExists)
            {
                ViewBag.ErrorMessage = "This username is already taken.";
                return View(model);
            }

            _service.AddUser(model.Username, model.Password, "Admin");

            TempData["SuccessMessage"] = "Admin account has been created. You can now log in!";
            return RedirectToAction("SignInAdmin");  
        }
    }
}
