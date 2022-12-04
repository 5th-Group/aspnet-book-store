using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IHelpers _helperRepository;
        int PAGE_SIZE = 6;
        public BookController(IBookRepository bookRepository, IHelpers helpersRepository)
        {
            _bookRepository = bookRepository;
            _helperRepository = helpersRepository;
        }
        [HttpGet("Books")]
        public IActionResult Index(string filter = "_", int? pageNumber = 1)
        {
            var bookList = _bookRepository.GetAll(filter).Select(book => new AddBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                PageCount = book.PageCount,
                Author = book.Author,
                Language = book.Language,
                Genre = book.Genre,
                Type = book.Type.ToArray(),
                CreatedAt = book.CreatedAt,
                ImageUri = book.ImageUri,
                ImageName = book.ImageName,
                SignedUrl = _helperRepository.GenerateSignedUrl(book.ImageName).Result,
                PublishDate = book.PublishDate,
                Publisher = book.Publisher,
                Isbn = book.Isbn,
                Description = book.Description
            });



            // var result = PaginatedList<BookViewModel>.Create(bookList.ToList(), pageNumber ?? 1, PAGE_SIZE);
            // if (!result.Any())
            // {
            //     ViewBag.Temp = "Not found";
            // }
            // return Ok(result);
            return View();
        }

        [HttpGet("Book/{bookId}")]
        public IActionResult Detail(string bookId)
        {
            var book = _bookRepository.GetById(bookId);
            return View(book);
        }

        [HttpGet("Book/Cart")]
        public IActionResult Cart()
        {
            return View();
        }
    }
}