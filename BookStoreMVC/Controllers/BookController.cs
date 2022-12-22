using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using BookStoreMVC.Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BookStoreMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IProductRepository _productRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHelpers _helpersRepository;


        private readonly ProjectionDefinition<Product> _productProjectionDefinition = Builders<Product>.Projection
            .Exclude(doc => doc.Reviews);

        private readonly ProjectionDefinition<Book> _bookProjectionDef = Builders<Book>.Projection
            .Exclude(doc => doc.Isbn)
            .Exclude(doc => doc.ImageUri)
            .Exclude(doc => doc.PageCount)
            .Exclude(doc => doc.PublishDate);

        int PAGE_SIZE = 6;

        private IEnumerable<string>? Headers = null!;

        public BookController(IBookRepository bookRepository, IHelpers helpersRepository,
            IAuthorRepository authorRepository, IProductRepository productRepository,
            IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository,
            UserManager<User> userManager, IReviewRepository reviewRepository, ILanguageRepository languageRepository)
        {
            _bookRepository = bookRepository;
            _helpersRepository = helpersRepository;
            _authorRepository = authorRepository;
            _productRepository = productRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
            _userManager = userManager;
            _reviewRepository = reviewRepository;
            _languageRepository = languageRepository;
        }

        [HttpGet("books")]
        public IActionResult Index(string filterBy = "", string filterValue = "", string filterOrder = "asc",
            int? pageNumber = 1)
        {
            List<ProductViewModel> productList;

            ViewBag.Category = _bookGenreRepository.GetAll();


            if (string.IsNullOrEmpty(filterBy) || string.IsNullOrEmpty(filterValue))
            {
                productList = _productRepository.GetAll(_productProjectionDefinition).Select(product =>
                {
                    var book = _bookRepository.GetById(product.BookId, _bookProjectionDef).Result;
                    var author = _authorRepository.GetById(book.Author).Result;
                    var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
                    var publisher = _publisherRepository.GetById(book.Publisher);
                    var lang = _languageRepository.GetByIdAsync(book.Language).Result;

                    var bookViewModel =
                        MapBook.MapIndexBookViewModel(book, author, bookGenres, publisher, lang, _helpersRepository);

                    return new ProductViewModel
                    {
                        Id = product.Id,
                        Price = product.CurrentPrice,
                        Rating = Convert.ToInt32(product.AverageScore),
                        Book = bookViewModel
                    };
                }).ToList();
            }
            else
            {
                productList = _productRepository.GetAll(_productProjectionDefinition).Select(product =>
                {
                    var book = _bookRepository.GetById(product.BookId).Result;
                    var author = _authorRepository.GetById(book.Author).Result;
                    var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
                    var publisher = _publisherRepository.GetById(book.Publisher);
                    var lang = _languageRepository.GetByIdAsync(book.Language).Result;

                    var bookViewModel = MapBook.MapIndexBookViewModel(book, author, bookGenres, publisher, lang,
                        _helpersRepository);

                    return new ProductViewModel
                    {
                        Id = product.Id,
                        Price = product.CurrentPrice,
                        Rating = Convert.ToInt32(product.AverageScore),
                        Book = bookViewModel
                    };
                }).Where(product =>
                {
                    return filterBy switch
                    {
                        "genre" => product.Book.Genre.Any(genre => genre.Name.Equals(filterValue)),
                        "title" => product.Book.Title.Equals(filterValue),
                        _ => true
                    };
                }).ToList();
            }


            var result =
                PaginatedList<ProductViewModel>.Create(productList, pageNumber ?? 1, PAGE_SIZE, Headers, "bookList");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }

            return View(result);
        }

        [HttpGet("book/{productId}/{productName}")]
        public async Task<IActionResult> Detail(string productId, string productName)
        {
            var product = _productRepository.GetById(productId);
            var book = await _bookRepository.GetById(product.BookId);
            var author = _authorRepository.GetById(book.Author).Result;
            var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
            var publisher = _publisherRepository.GetById(book.Publisher!);
            var lang = _languageRepository.GetByIdAsync(book.Language).Result;
            var reviews = product.Reviews?.Select(r => _reviewRepository.GetById(r));

            var productDetail = new ProductDetailViewModel
            {
                Id = product.Id,
                Book = MapBook.MapDetailBookViewModel(book, author, bookGenres, publisher, lang, _helpersRepository),
                Price = product.CurrentPrice,
                Rating = Convert.ToInt32(product.AverageScore),
                Reviews = reviews is null
                    ? new List<ReviewViewModel>()
                    : MapReview.MapReviewViewModels(reviews, _userManager)
            };

            return View(productDetail);
        }

        [HttpGet("book/cart")]
        public IActionResult Cart()
        {
            return View();
        }
        
        
        [HttpGet("books/api")]
        public JsonResult Api(string filterBy = "", string filterValue = "", string filterOrder = "asc",
            int? pageNumber = 1)
        {
            List<ProductViewModel> productList;

            ViewBag.Category = _bookGenreRepository.GetAll();


            if (string.IsNullOrEmpty(filterBy) || string.IsNullOrEmpty(filterValue))
            {
                productList = _productRepository.GetAll(_productProjectionDefinition).Select(product =>
                {
                    var book = _bookRepository.GetById(product.BookId, _bookProjectionDef).Result;
                    var author = _authorRepository.GetById(book.Author).Result;
                    var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
                    var publisher = _publisherRepository.GetById(book.Publisher);
                    var lang = _languageRepository.GetByIdAsync(book.Language).Result;

                    var bookViewModel =
                        MapBook.MapIndexBookViewModel(book, author, bookGenres, publisher, lang, _helpersRepository);

                    return new ProductViewModel
                    {
                        Id = product.Id,
                        Price = product.CurrentPrice,
                        Rating = Convert.ToInt32(product.AverageScore),
                        Book = bookViewModel
                    };
                }).ToList();
            }
            else
            {
                productList = _productRepository.GetAll(_productProjectionDefinition).Select(product =>
                {
                    var book = _bookRepository.GetById(product.BookId).Result;
                    var author = _authorRepository.GetById(book.Author).Result;
                    var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
                    var publisher = _publisherRepository.GetById(book.Publisher);
                    var lang = _languageRepository.GetByIdAsync(book.Language).Result;

                    var bookViewModel = MapBook.MapIndexBookViewModel(book, author, bookGenres, publisher, lang,
                        _helpersRepository);

                    return new ProductViewModel
                    {
                        Id = product.Id,
                        Price = product.CurrentPrice,
                        Rating = Convert.ToInt32(product.AverageScore),
                        Book = bookViewModel
                    };
                }).Where(product =>
                {
                    return filterBy switch
                    {
                        "genre" => product.Book.Genre.Any(genre => genre.Name.Equals(filterValue)),
                        "title" => product.Book.Title.Equals(filterValue),
                        _ => true
                    };
                }).ToList();
            }


            var result =
                PaginatedList<ProductViewModel>.Create(productList, pageNumber ?? 1, PAGE_SIZE, Headers, "bookList");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }

            return Json(result);
        }
    }
}