using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class AdminController : Controller
    {
        private IAuthorRepository _authorRepository;
        
        public AdminController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AddAuthor()
        {
            var model = new AuthorViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor(AuthorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var authorModel = new Author()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Initials = model.Initials,
                Description = model.Description
            };
            await _authorRepository.AddAsync(authorModel);

            return RedirectToAction("Index", "Admin");
        }
    }
}