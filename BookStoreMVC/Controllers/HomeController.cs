﻿using System.Diagnostics;
using System.Net;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace BookStoreMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden)
            return RedirectToAction("Login", "Authentication");

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #endregion

}