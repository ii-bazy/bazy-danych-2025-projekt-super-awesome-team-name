using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Store.DB;
using Online_Store.Models;
using System.Security.Claims;

namespace Online_Store.Controllers
{
    public class AccountController : Controller
    {
        public AccountController() { }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View(new ViewUser());
        }

        [HttpPost]
        public IActionResult SignIn(ViewAccount model)
        {
            // TODO: Get real user
            var user = new ViewAccount(){ Username = model.Username, Password = model.Password};

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
                    IsPersistent = false // Saves iven after clowsing brawser
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
            return View(new ViewUser());
        }

        [HttpPost]
        public IActionResult SignInAdmin(ViewAccount model)
        {
            // TODO: Get real admin
            var admin = new ViewAccount() { Username = model.Username, Password = model.Password };

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
            return View(new ViewUser());
        }

        [HttpPost]
        public IActionResult SignUp(ViewAccount model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.ErrorMessage = "Username and password cannot be empty.";
                return View(model);
            }

            // TODO: Check if given username is taken
            bool userExists = false;

            if (userExists)
            {
                ViewBag.ErrorMessage = "This username is already taken.";
                return View(model);
            }

            var user = new ViewAccount
            {
                Username = model.Username,
                Password = model.Password,
            };

            // TODO: Add new user to database

            TempData["SuccessMessage"] = "Your account has been created. You can now log in!";
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult SignUpAdmin()
        {
            ViewBag.ErrorMessage = null;
            return View(new ViewUser());
        }



        [HttpPost]
        public IActionResult SignUpAdmin(ViewAccount model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.ErrorMessage = "Username and password cannot be empty.";
                return View(model);
            }

            // TODO: Check if given username is taken
            bool userExists = false;

            if (userExists)
            {
                ViewBag.ErrorMessage = "This username is already taken.";
                return View(model);
            }

            var user = new ViewAccount
            {
                Username = model.Username,
                Password = model.Password
            };

            // TODO: Add new admin to database

            TempData["SuccessMessage"] = "Admin account has been created. You can now log in!";
            return RedirectToAction("SignInAdmin");  
        }
    }
}
