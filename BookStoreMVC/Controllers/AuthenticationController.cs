using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(UserManager<User> userManager, ILogger<AuthenticationController> logger, SignInManager<User> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region User Authentication

        [HttpGet]
        public IActionResult Login() => View();


        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userByUsername = await _userManager.FindByNameAsync(model.Username);
            var userByEmail = await _userManager.FindByNameAsync(model.Username);

            if (userByUsername != null && userByEmail != null) ViewData["Message"] = "User does not exist.";

            var result = await _signInManager.PasswordSignInAsync((userByEmail ?? userByUsername)!, model.Password, true, false);

            if (!result.Succeeded) ViewData["Message"] = "Username or password is incorrect";

            _logger.LogInformation($"User {userByEmail?.Username ?? userByUsername?.Username} has logged in at {DateTime.Now}");
            ViewData["Message"] = "Logged in successfully";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Data mapping
            var user = new User
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Address = model.Address,
                Email = model.Email,
                // Roles = model.Role,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber
            };

            // Create user async
            var result = await _userManager.CreateAsync(user, user.Password);

            // Add user to role
            await _userManager.AddToRoleAsync(user, "User");

            // Error handling
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            ViewData["Message"] = "Account have been created successfully.";

            return RedirectToAction("Index", "Home");
        }



        #endregion
    }
}