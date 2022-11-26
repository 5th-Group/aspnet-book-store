using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;

        private readonly ILanguageRepository _languageRepository;
        private readonly ICloudStorage _cloudStorage;
        int PAGE_SIZE = 5;

        public AdminController(ILanguageRepository languageRepository, IAuthorRepository authorRepository, IBookRepository bookRepository, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository, ICloudStorage cloudStorage)
        {
            _languageRepository = languageRepository;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
            _cloudStorage = cloudStorage;

        }

        public IActionResult Index()
        {
            return View();
        }

        #region Book

        [HttpGet("Admin/Books/")]
        public IActionResult BookIndex(string filter = "_", int? pageNumber = 1)
        {
            var bookList = _bookRepository.GetAll(filter).Select(book => new BookViewModel
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
                SignedUrl = GenerateSignedUrl(book.ImageName).Result,
                PublishDate = book.PublishDate,
                Publisher = book.Publisher,
                Isbn = book.Isbn,
                Description = book.Description
            });



            var result = PaginatedList<BookViewModel>.Create(bookList.ToList(), pageNumber ?? 1, PAGE_SIZE);
            return View(result);
        }



        [HttpGet]
        public IActionResult AddBook()
        {
            var bookModel = new BookViewModel();
            return View(bookModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookViewModel book)
        {
            if (!ModelState.IsValid) return View(book);

            if (book.Img != null)
            {
                book.ImageName = GenerateFileName(book.Img.FileName);
                book.ImageUri = await _cloudStorage.UploadFileAsync(book.Img, book.ImageName);
            }

            var model = new Book
            {
                Title = book.Title,
                PageCount = book.PageCount,
                Author = book.Author,
                Language = book.Language,
                Genre = book.Genre,
                Type = book.Type,
                ImageUri = book.ImageUri ?? string.Empty,
                ImageName = book.ImageName ?? string.Empty,
                PublishDate = book.PublishDate,
                Publisher = book.Publisher,
                Isbn = book.Isbn ?? string.Empty,
                Description = book.Description ?? string.Empty,
            };

            await _bookRepository.AddAsync(model);
            return RedirectToAction("BookIndex");

        }

        private string GenerateFileName(string imgFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(imgFileName);
            var fileExtension = Path.GetExtension(imgFileName);

            return $"{fileName}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}{fileExtension}";
        }

        private async Task<string> GenerateSignedUrl(string imgname)
        {
            if (string.IsNullOrWhiteSpace(imgname)) return string.Empty;


            var signedUrl = await _cloudStorage.GetSignedUrlAsync(imgname);

            return signedUrl;
        }

        #endregion

        #region Author

        public IActionResult AuthorIndex()
        {
            var authorList = _authorRepository.GetAll().Select(author => new AuthorViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                Initials = author.Initials,
                Description = author.Description,
            }).ToList();
            return View(authorList);
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

        #endregion

        #region BookGenre

        [HttpGet]
        public IActionResult BookGenreIndex()
        {
            var bookGenreList = _bookGenreRepository.GetAll().Select(bookGenre => new BookGenreViewModel
            {
                Id = bookGenre.Id,
                Name = bookGenre.Name
            });

            return View(bookGenreList.ToList());
        }

        [HttpGet]
        public IActionResult AddBookGenre()
        {
            var model = new BookGenreViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBookGenre(BookGenreViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var bookGenreModel = new BookGenre
            {
                Id = model.Id,
                Name = model.Name
            };

            await _bookGenreRepository.AddAsync(bookGenreModel);

            return RedirectToAction("BookGenreIndex");
        }
        #endregion


        #region Publisher

        [HttpGet]
        public IActionResult PublisherIndex()
        {
            var publishers = _publisherRepository.GetAll().ToArray();
            return View(publishers);
        }

        [HttpGet]
        public IActionResult AddPublisher()
        {
            var model = new PublisherViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddPublisher(PublisherViewModel publisher)
        {
            if (!ModelState.IsValid) return View(publisher);
            var document = new Publisher
            {
                Name = publisher.Name,
                Contact = publisher.Contact,
                Origin = publisher.Origin
            };

            await _publisherRepository.AddAsync(document);
            return RedirectToAction("PublisherIndex");
        }

        #endregion

        #region Language
        [HttpGet]
        public IActionResult LanguageIndex()
        {
            var publishers = _languageRepository.GetAll().ToArray();
            return View(publishers);
        }

        [HttpGet]
        public IActionResult AddLanguage()
        {
            var model = new LanguageViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddLanguage(LanguageViewModel language)
        {
            if (!ModelState.IsValid) return View(language);
            var document = new Language()
            {
                Id = language.Id,
                Name = language.Name,
                Code = language.Code,

            };

            await _languageRepository.AddAsync(document);
            return RedirectToAction("PublisherIndex");
        }
        #endregion
    }
}