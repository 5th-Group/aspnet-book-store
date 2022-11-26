using BookStoreMVC.Models;
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
        [HttpGet("Books")]
        public IActionResult Index(string filter = "_")
        {
            var books = _repository.GetAll(filter);
            return View(books);
        }

        [HttpGet("Book/{bookId}")]
        public IActionResult Detail(string bookId)
        {
            var book = _repository.GetById(bookId);
            return View(book);
        }

        [HttpGet("Book/Cart")]
        public IActionResult Cart()
        {
            return View();
        }
    }
}