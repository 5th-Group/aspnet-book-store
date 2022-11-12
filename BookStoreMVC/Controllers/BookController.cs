using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _repository;
        public BookController(IBookRepository bookRepository)
        {
            _repository = bookRepository;
        }
        public IActionResult Index()
        {
            var books = _repository.GetAll();
            

            return View(books);
        }
    }
}