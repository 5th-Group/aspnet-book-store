using System.Diagnostics;
using System.Net;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace BookStoreMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<User> _userManager;



    public HomeController(UserManager<User> userManager, IOrderRepository orderRepository, ILogger<HomeController> logger)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    #region Basic

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpGet("/checkorder")]
    public IActionResult CheckOrder()
    {
        return View();
    }
    // [HttpGet("/orderdetail/{orderId}")]
    public async Task<IActionResult> OrderDetail(string orderId)
    {
        var order = _orderRepository.GetByOrderId(orderId).Result;
        var user = _userManager.FindByIdAsync(order.Customer).Result;
        var userVM = new UserDetailViewModel();

        userVM.Address = user.Address.ToString();
        userVM.Country = user.Country;
        userVM.Email = user.Email;
        userVM.Firstname = user.FirstName;
        userVM.Lastname = user.LastName;
        userVM.Gender = user.Gender;
        userVM.PhoneNumber = user.PhoneNumber;
        userVM.Username = user.UserName;

        var orderVM = new OrderIndexViewModel
        {
            CreatedAt = order.CreatedAt,
            Customer = userVM,
            Id = order.Id,
            PaymentStatus = order.PaymentStatus,
            TotalPrice = order.TotalPrice,
            ShippingStatus = order.CurrentShippingStatus
        };
        return View(orderVM);
    }


    public IActionResult Payment()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden)
            return RedirectToAction("Login", "Authentication");

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #endregion

}