using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using BookStoreMVC.ViewModels.Book;
using Microsoft.AspNetCore.Identity;
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
        private readonly IHelpers _helpersRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        private const int PAGE_SIZE = 10;
        private IEnumerable<string>? Headers = null!;


        public AdminController(IHelpers helpersRepository, ILanguageRepository languageRepository, IAuthorRepository authorRepository, IBookRepository bookRepository, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository, ICloudStorage cloudStorage, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _languageRepository = languageRepository;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
            _cloudStorage = cloudStorage;
            _userManager = userManager;
            _roleManager = roleManager;
            _helpersRepository = helpersRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Book

        [HttpGet("Admin/Books/")]
        public IActionResult BookIndex(string filter = "_", int? pageNumber = 1)
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

            Headers = PropertiesFromType(bookList);



            var result = PaginatedList<IndexBookViewModel>.Create(bookList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "BookIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }

            return View(result);
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            var authors = _authorRepository.GetAll();
            var genres = _bookGenreRepository.GetAll();
            var languages = _languageRepository.GetAll();
            var publishers = _publisherRepository.GetAll();

            var bookModel = new AddBookViewModel
            {
                AuthorsList = authors,
                GenreList = genres,
                LanguageList = languages,
                PublisherList = publishers
            };

            return View(bookModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookViewModel book)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
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


        [HttpPost]
        public async Task<IActionResult> DeleteBook(string bookId)
        {
            await _bookRepository.DeleteAsync(bookId);
            return RedirectToAction("BookIndex");
        }
        private string GenerateFileName(string imgFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(imgFileName);
            var fileExtension = Path.GetExtension(imgFileName);

            return $"{fileName}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}{fileExtension}";
        }

        public IEnumerable<string> PropertiesFromType<T>(IEnumerable<T> input)
        {
            var item = input.First();
            var properties = new List<string>();

            foreach (var item2 in item.GetType().GetProperties())
            {
                properties.Add(item2.Name);
            }

            return properties;
        }



        #endregion

        #region Author

        public IActionResult AuthorIndex(int? pageNumber = 1)
        {
            var authorList = _authorRepository.GetAll().Select(author => new AuthorViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                Initials = author.Initials,
                Description = author.Description,
            });

            Headers = PropertiesFromType(authorList);
            var result = PaginatedList<AuthorViewModel>.Create(authorList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "AuthorIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);

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

        public async Task<IActionResult> DeleteAuthor(string authorId)
        {
            await _authorRepository.DeleteAsync(authorId);
            return RedirectToAction("AuthorIndex");
        }

        #endregion

        #region BookGenre


        public IActionResult BookGenreIndex(int? pageNumber = 1)
        {
            var bookGenreList = _bookGenreRepository.GetAll().Select(bookGenre => new BookGenreViewModel
            {
                Id = bookGenre.Id,
                Name = bookGenre.Name,
            });

            Headers = PropertiesFromType(bookGenreList);
            var result = PaginatedList<BookGenreViewModel>.Create(bookGenreList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "BookGenreIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);
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


        [HttpPost]
        public async Task<IActionResult> DeleteBookGenre(string bookGenreID)
        {
            await _bookGenreRepository.DeleteAsync(bookGenreID);
            return RedirectToAction("BookGenreIndex");
        }
        #endregion

        #region Publisher

        [HttpGet]
        public IActionResult PublisherIndex(int? pageNumber)
        {
            var publishers = _publisherRepository.GetAll().Select(publisher => new PublisherViewModel
            {
                Id = publisher.Id,
                Contact = publisher.Contact,
                Name = publisher.Name,
                Origin = publisher.Origin
            });


            Headers = PropertiesFromType(publishers);
            var result = PaginatedList<PublisherViewModel>.Create(publishers.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "PublisherIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);
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

        public async Task<IActionResult> DeletePublisher(string publisherId)
        {
            await _publisherRepository.DeleteAsync(publisherId);
            return RedirectToAction("PublisherIndex");
        }
        #endregion

        #region Language
        [HttpGet]
        public IActionResult LanguageIndex(int? pageNumber = 1)
        {
            var languageList = _languageRepository.GetAll().Select(language => new LanguageViewModel
            {
                Id = language.Id,
                Code = language.Code,
                Name = language.Name
            });

            Headers = PropertiesFromType(languageList);
            var result = PaginatedList<LanguageViewModel>.Create(languageList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "LanguageIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);


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


        public async Task<IActionResult> DeleteLanguage(string languageID)
        {
            await _languageRepository.DeleteAsync(languageID);
            return RedirectToAction("LanguageIndex");
        }
        #endregion

        #region User Management

        [HttpGet]
        public IActionResult UserIndex()
        {
            var userList = _userManager.Users.ToList();

            return View(userList);
        }

        [HttpGet]
        public IActionResult Login()
        {


            return View();
        }

        [HttpGet]
        public IActionResult AddIdentity() => View();


        // public async Task<IActionResult> AddIdentity(AdminAddIdentityViewModel model)
        // {
        //     return RedirectToAction();
        // }


        #endregion

        #region Role Management

        [HttpGet]
        public IActionResult RoleIndex()
        {
            var rolesList = _roleManager.Roles.ToList();
            return View(rolesList);
        }

        [HttpGet]
        public IActionResult AddRole() => View();

        [HttpPost]
        public async Task<IActionResult> AddRole(Role model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _roleManager.CreateAsync(new Role
            {
                Name = model.Name,
            });

            if (result.Succeeded)
            {
                ViewData["Message"] = $"Role {model.Name} added successfully.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            return RedirectToAction("RoleIndex");
        }

        #endregion
    }
}