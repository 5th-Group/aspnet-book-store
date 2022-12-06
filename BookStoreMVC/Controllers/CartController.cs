using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{

    public class CartController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHelpers _helpersRepository;

        public const string CARTKEY = "cart";
        public CartController(IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _helpersRepository = helpersRepository;
        }
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(HttpContext.Session, CARTKEY);
            var shoppingCart = new ShoppingCart
            {
                ShoppingCartItems = cart
            };
            // ViewBag.total = cart.ShoppingCartItems.Sum(item => item.Book.Price * item.Quantity);
            if (shoppingCart != null)
            {
                return View(shoppingCart);

            }
            return View();
        }


        public async Task<IActionResult> AddToCart(string id, string decs, string incs)
        {

            if (SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(HttpContext.Session, CARTKEY) == null)
            {
                List<ShoppingCartItem> cart = new List<ShoppingCartItem>();
                Book b = _bookRepository.GetById(id).Result;

                var book = new IndexBookViewModel();

                book.Id = b.Id;
                book.Title = b.Title;
                book.AuthorDisplay = _authorRepository.GetById(b.Author).Result;
                book.SignedUrl = _helpersRepository.GenerateSignedUrl(b.ImageName).Result;

                if (book == null)
                {
                    return NotFound();

                }
                cart.Add(new ShoppingCartItem { Book = book, Amount = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);

            }
            else
            {
                var cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(HttpContext.Session, CARTKEY);
                int index = isExist(id);
                if (index != -1)
                {
                    if (decs == "decs")
                    {
                        cart[index].Amount--;
                        SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);
                        return RedirectToAction("Index");
                    }
                    else if (incs == "incs")

                    {
                        cart[index].Amount++;
                        SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);
                        return RedirectToAction("Index", cart);

                    }
                    cart[index].Amount++;


                }
                else
                {
                    Book b = await _bookRepository.GetById(id);
                    IndexBookViewModel book = new IndexBookViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        PageCount = b.PageCount,
                        AuthorDisplay = _authorRepository.GetById(b.Author).Result,
                        Language = b.Language,
                        Genre = b.Genre,
                        Type = b.Type.ToArray(),
                        CreatedAt = b.CreatedAt,
                        ImageName = b.ImageName,
                        SignedUrl = _helpersRepository.GenerateSignedUrl(b.ImageName).Result,
                        PublishDate = b.PublishDate,
                        Publisher = b.Publisher,
                        Isbn = b.Isbn,
                        Description = b.Description
                    };
                    cart.Add(new ShoppingCartItem { Book = book, Amount = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);
            }
            return RedirectToAction("Index", "Book");
        }

        [HttpPost]
        public IActionResult Remove(string id)
        {
            List<ShoppingCartItem> cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(HttpContext.Session, CARTKEY);
            int index = isExist(id);
            if (index != -1)
            {
                cart.RemoveAt(index);
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);
            return RedirectToAction("Index");
        }


        private int isExist(string id)
        {
            List<ShoppingCartItem> cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(HttpContext.Session, CARTKEY);
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Book.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}