using System.Diagnostics;
using System.Security.Claims;
using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.Controllers;

public class UserController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;


    public UserController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    #region Basic
    [Authorize("RequireUserRole")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        var userVm = new UserDetailViewModel
        {
            Address = user.Address.ToString(),
            Country = user.Country,
            Email = user.Email,
            Firstname = user.FirstName,
            Lastname = user.LastName,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            Username = user.UserName
        };
        return View(userVm);
    }

    [Authorize("RequireUserRole")]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }
    [Authorize("RequireUserRole")]
    [HttpGet]
    public IActionResult OrderHistory()
    {
        return View();
    }





    #endregion

}