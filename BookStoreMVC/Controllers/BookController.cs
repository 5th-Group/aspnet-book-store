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
        private readonly IProductRepository _productRepository;


        int PAGE_SIZE = 6;

        private IEnumerable<string>? Headers = null!;
        public BookController(IBookRepository bookRepository, IHelpers helpersRepository, IAuthorRepository authorRepository, IProductRepository productRepository)
        {
            _bookRepository = bookRepository;
            _helpersRepository = helpersRepository;
            _authorRepository = authorRepository;
            _productRepository = productRepository;
        }

        [HttpGet("Books")]
        public IActionResult Index(string filter = "_", int? pageNumber = 1)
        {

            var productList = _productRepository.GetAll().Select(product =>
            {
                var book = _bookRepository.GetById(product.BookId).Result;

                var bookViewModel = new IndexBookViewModel();

                bookViewModel.Id = book.Id;
                bookViewModel.Title = book.Title;
                bookViewModel.PageCount = book.PageCount;
                bookViewModel.AuthorDisplay = _authorRepository.GetById(book.Author).Result;
                bookViewModel.Language = book.Language;
                bookViewModel.Genre = book.Genre;
                bookViewModel.Type = book.Type.ToArray();
                bookViewModel.CreatedAt = book.CreatedAt;
                bookViewModel.ImageName = book.ImageName;
                bookViewModel.SignedUrl = _helpersRepository.GenerateSignedUrl(book.ImageName).Result;
                bookViewModel.PublishDate = book.PublishDate;
                bookViewModel.Publisher = book.Publisher;
                bookViewModel.Isbn = book.Isbn;
                bookViewModel.Description = book.Description;

                return new ProductViewModel
                {
                    Id = product.Id,
                    Price = product.CurrentPrice,
                    Rating = product.AverageScore,
                    Book = bookViewModel

                };
            });


            var result = PaginatedList<ProductViewModel>.Create(productList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "bookList");
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