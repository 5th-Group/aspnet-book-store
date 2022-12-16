
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreMVC.Controllers;

public class UserController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IOrderRepository _orderRepository;


    public UserController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IOrderRepository orderRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public string GetCartKey()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        var model = new ChangePasswordViewModel();
        return View(model);
    }
    [Authorize("RequireUserRole")]
    [HttpGet]
    public IActionResult OrderHistory()
    {
        IEnumerable<Order> orderList = _orderRepository.GetByUserId(GetCartKey());

        return View(orderList);
    }






    #endregion

}