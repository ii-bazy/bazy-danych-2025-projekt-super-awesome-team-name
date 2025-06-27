using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Store.Models;
using System.Security.Claims;
using Online_Store.Services;
using Microsoft.AspNetCore.Identity;

namespace Online_Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly IService _service;
        private readonly PasswordHasher<string> _passwordHasher = new();

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
            var user = _service.GetByUsername(model.Username);

            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(model.Username, user.Password.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Success)
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
                    ViewBag.ErrorMessage = "Invalid password.";
                    return View(model);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username.";
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
            var admin = _service.GetByUsername(model.Username);

            if (admin != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(model.Username, admin.Password.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Success)
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

            string hashedPassword = _passwordHasher.HashPassword(model.Username, model.Password);
            _service.AddUser(model.Username, hashedPassword, "Customer");

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

            string hashedPassword = _passwordHasher.HashPassword(model.Username, model.Password);
            _service.AddUser(model.Username, hashedPassword, "Admin");

            TempData["SuccessMessage"] = "Admin account has been created. You can now log in!";
            return RedirectToAction("SignInAdmin");  
        }
    }
}
