using BookStoreMVC.Mapper;
using BookStoreMVC.Models;
using BookStoreMVC.Models.Payment;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using BookStoreMVC.ViewModels.Admin;
using BookStoreMVC.ViewModels.Book;
using BookStoreMVC.ViewModels.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository.Utils;

namespace BookStoreMVC.Controllers
{
    // [Authorize("RequireAdminRole")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICloudStorage _cloudStorage;
        private readonly IHelpers _helpersRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IOrderRepository _orderRepository;

        private const int PAGE_SIZE = 10;
        private IEnumerable<string>? Headers;


        public AdminController(IOrderRepository orderRepository, IHelpers helpersRepository, ILanguageRepository languageRepository, IAuthorRepository authorRepository, IBookRepository bookRepository, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository, ICloudStorage cloudStorage, UserManager<User> userManager, RoleManager<Role> roleManager, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _languageRepository = languageRepository;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
            _cloudStorage = cloudStorage;
            _userManager = userManager;
            _roleManager = roleManager;
            _productRepository = productRepository;
            _helpersRepository = helpersRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        #region Book

        [HttpGet("books/")]
        public IActionResult BookIndex(string filter = "_", int? pageNumber = 1)
        {
            var bookList = MapBook.MapIndexBookViewModels(
                _bookRepository.GetAll(),
                _authorRepository,
                _bookGenreRepository,
                _publisherRepository,
                _languageRepository,
                _helpersRepository).ToList();


            if (!bookList.Any())
            {
                ViewBag.Temp = "Not found";
                return View();
            }

            Headers = PropertiesFromType(bookList);

            var result = PaginatedList<IndexBookViewModel>.Create(bookList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "BookIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }

            return View(result);
        }

        [HttpGet("add-book")]
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

        [HttpPost("add-book")]
        public async Task<IActionResult> AddBook(AddBookViewModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            if (model.Img != null)
            {
                model.ImageName = GenerateFileName(model.Img.FileName);
                model.ImageUri = await _cloudStorage.UploadFileAsync(model.Img, model.ImageName);
            }

            var book = new Book
            {
                Id = IdGenerator.GetId<ObjectId>().ToString(),
                Title = model.Title,
                PageCount = model.PageCount,
                Author = model.Author,
                Language = model.Language,
                Genre = model.Genre,
                Type = model.Type,
                ImageUri = model.ImageUri ?? string.Empty,
                ImageName = model.ImageName ?? string.Empty,
                PublishDate = model.PublishDate,
                Publisher = model.Publisher,
                Isbn = model.Isbn ?? string.Empty,
                Description = model.Description ?? string.Empty,
            };

            // var doc = BsonDocument.Create(model);
            await _bookRepository.AddAsync(book);

            var product = new Product
            {
                BookId = book.Id,
                BasePrice = new PriceStruct
                {
                    Hardcover = model.HardcoverPrice,
                    Paperback = model.PaperbackPrice,
                    Ebook = model.EbookPrice
                },
                CurrentPrice = new PriceStruct
                {
                    Hardcover = model.HardcoverPrice,
                    Paperback = model.PaperbackPrice,
                    Ebook = model.EbookPrice
                },
                CreatedDate = DateTime.Now,
                ExpireDate = DateTime.Now,
                AverageScore = 0
            };

            await _productRepository.AddAsync(product);

            return RedirectToAction("BookIndex");

        }


        [HttpPost("book/delete")]
        public async Task<IActionResult> DeleteBook(string bookId)
        {
            await _bookRepository.DeleteAsync(bookId);
            return RedirectToAction("BookIndex");
        }

        private static string GenerateFileName(string imgFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(imgFileName);
            var fileExtension = Path.GetExtension(imgFileName);

            return $"{fileName}-{DateTime.Now.ToUniversalTime():yyyyMMddHHmmss}{fileExtension}";
        }

        private IEnumerable<string> PropertiesFromType<T>(IEnumerable<T> input)
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

        [HttpGet("authors")]
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
            if (authorList != null && authorList.Any())
            {

                Headers = PropertiesFromType(authorList);
            }
            var result = PaginatedList<AuthorViewModel>.Create(authorList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "AuthorIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);

        }

        [HttpGet("author/add")]
        public IActionResult AddAuthor()
        {
            var model = new AuthorViewModel();
            return View(model);
        }

        [HttpPost("author/add")]
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

        [HttpPost("author/delete")]
        public async Task<IActionResult> DeleteAuthor(string authorId)
        {
            await _authorRepository.DeleteAsync(authorId);
            return RedirectToAction("AuthorIndex");
        }

        #endregion

        #region BookGenre

        [HttpGet("book-genres")]
        public IActionResult BookGenreIndex(int? pageNumber = 1)
        {
            var bookGenreList = _bookGenreRepository.GetAll().Select(bookGenre => new BookGenreViewModel
            {
                Id = bookGenre.Id,
                Name = bookGenre.Name,
            });
            if (bookGenreList != null && bookGenreList.Any())
            {

                Headers = PropertiesFromType(bookGenreList);
            }
            var result = PaginatedList<BookGenreViewModel>.Create(bookGenreList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "BookGenreIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);
        }

        [HttpGet("book-genre/add")]
        public IActionResult AddBookGenre()
        {
            var model = new BookGenreViewModel();
            return View(model);
        }

        [HttpPost("book-genre/add")]
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


        [HttpPost("book-genre/delete")]
        public async Task<IActionResult> DeleteBookGenre(string bookGenreID)
        {
            await _bookGenreRepository.DeleteAsync(bookGenreID);
            return RedirectToAction("BookGenreIndex");
        }
        #endregion

        #region Publisher

        [HttpGet("publishers")]
        public IActionResult PublisherIndex(int? pageNumber, string filter)
        {
            var publishers = _publisherRepository.GetAll().Select(publisher => new PublisherViewModel
            {
                Id = publisher.Id,
                Contact = publisher.Contact,
                Name = publisher.Name,
                Origin = publisher.Origin
            });

            if (publishers.Any() && publishers != null)
            {
                Headers = PropertiesFromType(publishers);

            }

            var result = PaginatedList<PublisherViewModel>.Create(publishers.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "PublisherIndex");
            if (!result.Any() || result is null)
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);
        }

        [HttpGet("publisher/add")]
        public IActionResult AddPublisher()
        {
            var model = new PublisherViewModel();
            return View(model);
        }


        [HttpPost("publisher/add")]
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

        [HttpPost("publisher/delete")]
        public async Task<IActionResult> DeletePublisher(string publisherId)
        {
            await _publisherRepository.DeleteAsync(publisherId);
            return RedirectToAction("PublisherIndex");
        }
        #endregion

        #region Language
        [HttpGet("languages")]
        public IActionResult LanguageIndex(int? pageNumber = 1)
        {
            var languageList = _languageRepository.GetAll().Select(language => new LanguageViewModel
            {
                Id = language.Id,
                Code = language.Code,
                Name = language.Name
            });
            if (languageList != null && languageList.Any())
            {

                Headers = PropertiesFromType(languageList);
            }
            var result = PaginatedList<LanguageViewModel>.Create(languageList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "LanguageIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);


        }

        [HttpGet("language/add")]
        public IActionResult AddLanguage()
        {
            var model = new LanguageViewModel();
            return View(model);
        }


        [HttpPost("language/add")]
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

        [HttpPost("language/delete")]
        public async Task<IActionResult> DeleteLanguage(string languageId)
        {
            await _languageRepository.DeleteAsync(languageId);
            return RedirectToAction("LanguageIndex");
        }
        #endregion


        #region Order
        [HttpGet("orders")]
        public IActionResult OrderIndex(int? pageNumber = 1)
        {
            var orderList = _orderRepository.GetAll().Select(order =>
            {
                var user = _userManager.FindByIdAsync(order.Customer).Result;
                var userVM = new UserDetailViewModel
                {
                    Address = user.Address.ToString(),
                    Country = user.Country,
                    Email = user.Email,
                    Firstname = user.FirstName,
                    Lastname = user.LastName,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.UserName
                };


                return new OrderIndexViewModel
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    PaymentStatus = order.PaymentStatus,
                    ShippingStatus = order.CurrentShippingStatus,
                    ShippingStatusGroup = order.ShippingStatus.Select(s => new ShippingStatus
                    {
                        Name = s.Name,
                        Timestamp = s.TimeStamp
                    }),
                    TotalPrice = order.TotalPrice,
                    Customer = userVM
                };
            });


            var result = PaginatedList<OrderIndexViewModel>.Create(orderList.ToList(), pageNumber ?? 1, PAGE_SIZE, Headers, "OrderIndex");
            if (!result.Any())
            {
                return NotFound();
            }
            return View(result);


        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> OrderDetail(string orderId)
        {
            var order = await _orderRepository.GetByFilterAsync(Builders<Order>.Filter.Where(d => d.Id == orderId));

            return View(order);
        }

        [HttpPost("order/{orderId}/update-status")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, string name, string timeStamp)
        {
            var order = await _orderRepository.GetByFilterAsync(Builders<Order>.Filter.Where(d => d.Id == orderId));

            var orderStatus = order.ShippingStatus.Append(new OrderStatus
            {
                Name = name,
                TimeStamp = DateTime.Parse(timeStamp)
            });

            order.ShippingStatus = orderStatus;
            order.CurrentShippingStatus = order.ShippingStatus.Count() - 1;

            await _orderRepository.UpdateAsync(order);

            return RedirectToAction("OrderDetail", new { orderId = order.Id });
        }

        #endregion

        #region User Management

        [HttpGet("security/users")]
        public IActionResult UserIndex(int? pageNumber = 1)
        {
            var userList = _userManager.Users.ToList();

            var result = PaginatedList<User>.Create(userList, pageNumber ?? 1, PAGE_SIZE, Headers, "UserIndex");
            if (!result.Any())
            {
                ViewBag.Temp = "Not found";
            }
            return View(result);
        }

        // [HttpGet("security/edit-user")]
        // public async Task<IActionResult> EditUser(string username)
        // {
        //     var user = await _userManager.FindByNameAsync(username);
        //
        //     await _userManager.AddToRoleAsync(user, "ADMIN");
        //
        //     return RedirectToAction("Index");
        // }
        //

        // [Authorize("RequireAdminRole")]
        [HttpGet("security/add-user")]
        public IActionResult AddUser()
        {
            ViewBag.RolesList = _roleManager.Roles.ToArray();

            var model = new AddUserViewModel();

            return View(model);
        }

        // [Authorize("RequireAdminRole")]
        [HttpPost("security/add-user")]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            return RedirectToAction("UserIndex");
        }


        #endregion

        #region Role Management

        [HttpGet("security/roles")]
        public IActionResult RoleIndex()
        {
            var rolesList = _roleManager.Roles;
            return View(rolesList.ToList());
        }


        [HttpGet("security/add-role")]
        public IActionResult AddRole() => View();

        [HttpPost("security/add-role")]
        public async Task<IActionResult> AddRole(AddRoleViewModel model)
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