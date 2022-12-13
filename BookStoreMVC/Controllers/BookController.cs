using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using BookStoreMVC.ViewModels.Book;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IHelpers _helpersRepository;


        int PAGE_SIZE = 6;

        private IEnumerable<string>? Headers = null!;
        public BookController(IBookRepository bookRepository, IHelpers helpersRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _helpersRepository = helpersRepository;
            _authorRepository = authorRepository;
        }

        [HttpGet("Books")]
        public IActionResult Index(string filter = "_", int? pageNumber = 1)
        {
            var bookList = _bookRepository.GetAll(filter).Select(book => new IndexBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                PageCount = book.PageCount,
                AuthorDisplay = _authorRepository.GetById(book.Author).Result,
                Language = book.Language,
                Genre = book.Genre,
                Type = book.Type.ToArray(),
                CreatedAt = book.CreatedAt,
                ImageName = book.ImageName,
                SignedUrl = _helpersRepository.GenerateSignedUrl(book.ImageName).Result,
                PublishDate = book.PublishDate,
                Publisher = book.Publisher,
                Isbn = book.Isbn,
                Description = book.Description
            });



            var result = PaginatedList<IndexBookViewModel>.Create(bookList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "bookList");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }




            return View(result);
        }

        [HttpGet("Book/{bookId}")]
        public async Task<IActionResult> Detail(string bookId)
        {
            var book = await _bookRepository.GetById(bookId);
            return View(book);
        }

        [HttpGet("Book/Cart")]
        public IActionResult Cart()
        {
            return View();
        }
    }
}