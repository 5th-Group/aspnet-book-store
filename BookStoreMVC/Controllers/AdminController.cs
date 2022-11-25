using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        public AdminController(IAuthorRepository authorRepository, IBookRepository bookRepository, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        #region Book
        
        [HttpGet("Admin/Books/")]
        public IActionResult BookIndex(string filter = "_")
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
                PublishDate = book.PublishDate,
                Publisher = book.Publisher,
                Isbn = book.Isbn,
                Description = book.Description
            });

            return View(bookList.ToList());
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
            var model = new Book
            {
                Title = book.Title,
                PageCount = book.PageCount,
                Author = book.Author,
                Language = book.Language,
                Genre = book.Genre,
                Type = book.Type,
                ImageUri = book.ImageUri,
                PublishDate = book.PublishDate,
                Publisher = book.Publisher,
                Isbn = book.Isbn,
                Description = book.Description,
            };
            
            await _bookRepository.AddAsync(model);
            return RedirectToAction("BookIndex");

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
    }
}