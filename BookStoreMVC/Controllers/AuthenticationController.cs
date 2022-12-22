using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using BookStoreMVC.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(UserManager<User> userManager, ILogger<AuthenticationController> logger,
            SignInManager<User> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        #region User Authentication

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userByUsername = await _userManager.FindByNameAsync(model.Username);
            var userByEmail = await _userManager.FindByEmailAsync(model.Username);

            if (userByUsername is null && userByEmail is null)
            {
                ViewData["Message"] = "User does not exist.";
                return View(model);
            }

            var result =
                await _signInManager.PasswordSignInAsync((userByEmail ?? userByUsername)!, model.Password, false,
                    false);

            if (!result.Succeeded)
            {
                ViewData["Message"] = "Username or password is incorrect";
                return View(model);
            }

            _logger.LogInformation(string.Format("User {0} has logged in at {1}",
                userByEmail?.UserName ?? userByUsername?.UserName, DateTime.Now));
            ViewData["Message"] = "Logged in successfully";

            return RedirectToAction("Index", "Home");
        }


        // [Route("/register")]
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Data mapping
            var user = new User
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Address = new List<UserAddress>().Append(new UserAddress
                {
                    Type = model.AddressType,
                    Location = model.Address
                }),
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                // Roles = model.Role,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber,
            };

            // Create user async
            var result = await _userManager.CreateAsync(user, model.Password);

            // Error handling
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Add user to role
            await _userManager.AddToRoleAsync(user, "User");


            ViewData["Message"] = "Account have been created successfully.";

            return RedirectToAction("Index", "Home");
        }

        [Authorize("RequireUserRole")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}