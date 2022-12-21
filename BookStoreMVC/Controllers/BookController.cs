using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using BookStoreMVC.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IHelpers _helpersRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILanguageRepository _languageRepository;

        int PAGE_SIZE = 6;

        private IEnumerable<string>? Headers = null!;
        public BookController(ILanguageRepository languageRepository, IBookRepository bookRepository, IHelpers helpersRepository, IAuthorRepository authorRepository, IProductRepository productRepository, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository)
        {
            _languageRepository = languageRepository;
            _bookRepository = bookRepository;
            _helpersRepository = helpersRepository;
            _authorRepository = authorRepository;
            _productRepository = productRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
        }

        [HttpGet("Books")]
        public IActionResult Index(string filter = "_", int? pageNumber = 1)
        {

            var productList = _productRepository.GetAll().Select(product =>
            {
                var book = _bookRepository.GetById(product.BookId).Result;
                var author = _authorRepository.GetById(book.Id).Result;
                var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
                var publisher = _publisherRepository.GetById(book.Publisher);
                var language = _languageRepository.GetById(book.Language);

                // var bookViewModel = new IndexBookViewModel();
                //
                // bookViewModel.Id = book.Id;
                // bookViewModel.Title = book.Title;
                // bookViewModel.PageCount = book.PageCount;
                // bookViewModel.Author = _authorRepository.GetById(book.Author).Result;
                // bookViewModel.Language = book.Language;
                // bookViewModel.Genre = book.Genre;
                // bookViewModel.Type = book.Type.ToArray();
                // bookViewModel.CreatedAt = book.CreatedAt;
                // bookViewModel.ImageName = book.ImageName;
                // bookViewModel.SignedUrl = _helpersRepository.GenerateSignedUrl(book.ImageName).Result;
                // bookViewModel.PublishDate = book.PublishDate;
                // bookViewModel.Publisher = book.Publisher;
                // bookViewModel.Isbn = book.Isbn;
                // bookViewModel.Description = book.Description;

                var bookViewModel = BookMapper.MapBookViewModel(book, author, bookGenres, publisher, language, _helpersRepository);

                return new ProductViewModel
                {
                    Id = product.Id,
                    Price = product.CurrentPrice,
                    Rating = Convert.ToInt32(product.AverageScore),
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

        // [HttpGet("Book/{productId}/{productName}")]
        public async Task<IActionResult> Detail(string productId, string productName)
        {
            var product = _productRepository.GetById(productId);
            var book = await _bookRepository.GetById(product.BookId);
            var author = _authorRepository.GetById(book.Author).Result;
            var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
            var publisher = _publisherRepository.GetById(book.Publisher);
            var language = _languageRepository.GetById(book.Language);

            var bookViewModel = BookMapper.MapBookViewModel(book, author, bookGenres, publisher, language, _helpersRepository);

            var productViewModel = new ProductViewModel
            {
                Id = product.Id,
                Price = product.CurrentPrice,
                Rating = Convert.ToInt32(product.AverageScore),
                Book = bookViewModel

            };

            return View(productViewModel);
        }

        [HttpGet("Book/Cart")]
        public IActionResult Cart()
        {
            return View();
        }
    }
}