using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStoreMVC.Mapper;
using BookStoreMVC.Models.Cart;


namespace BookStoreMVC.Controllers
{

    public class CartController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHelpers _helpersRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly ILanguageRepository _languageRepository;

        private readonly UserManager<User> _userManager;

        private IList<ShoppingCartItem> _cartItemList = new List<ShoppingCartItem>();

        private const int PAGE_SIZE = 10;

        private IEnumerable<string>? Headers;

        public CartController(IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository, IProductRepository productRepository, UserManager<User> userManager, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository, ILanguageRepository languageRepository)
        {

            _productRepository = productRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _helpersRepository = helpersRepository;
            _userManager = userManager;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
            _languageRepository = languageRepository;
        }

        public string GetCartKey()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "GHOST_USR";
        }

        [HttpGet("cart")]
        public IActionResult Index(int? pageNumber = 1)
        {
            var key = GetCartKey();
            if (key != "GHOST_USR" && HttpContext.Session.Keys.Contains("GHOST_USR"))
            {
                HttpContext.Session.SetObjectAsJson(key,
                    HttpContext.Session.GetObjectFromJson<List<ProductListItem>>("GHOST_USR"));
                HttpContext.Session.Remove("GHOST_USR");
            }

            IEnumerable<ProductListItem> cart = HttpContext.Session.GetObjectFromJson<List<ProductListItem>>(key);

            if (cart != null && cart.Any())
            {
                foreach (var item in cart)
                {

                    var product = _productRepository.GetById(item.ProductDetail);
                    var book = _bookRepository.GetById(product.BookId).Result;
                    var author = _authorRepository.GetById(book.Author).Result;
                    var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre).Result);
                    var publisher = _publisherRepository.GetById(book.Publisher);
                    var lang = _languageRepository.GetByIdAsync(book.Language).Result;

                    var bookViewModel = MapBook.MapIndexBookViewModel(book, author, bookGenres, publisher, lang, _helpersRepository);



                    var productViewModel = new ProductViewModel
                    {
                        Id = product.Id,
                        Price = product.CurrentPrice,
                        Rating = Convert.ToInt32(product.AverageScore),
                        Book = bookViewModel
                    };

                    var cartItem = new ShoppingCartItem
                    {
                        Product = productViewModel,
                        Amount = item.Quantity,
                        Price = item.TotalPrice
                    };

                    _cartItemList.Add(cartItem);
                }
            }
            else
            {
                // if (cart is null)
                // {
                // return NotFound();
                // Generate Empty cart list so user can still access cart page instead of throwing error
                HttpContext.Session.SetString(GetCartKey(), string.Empty);
            }

            var result = PaginatedList<ShoppingCartItem>.Create(_cartItemList, pageNumber ?? 1, PAGE_SIZE, Headers, "CartIndex");
            return View(result);
        }

        [HttpPost("cart/add")]
        public IActionResult AddToCart(string id, string decs, string incs)
        {
            // if (string.IsNullOrEmpty(HttpContext.Session.GetString(GetCartKey())))
            // {
            //     var cart = new List<ProductListItem>();
            //     var product = _productRepository.GetById(id);
            //     
            //     cart.Add(new ProductListItem { ProductDetail = product.Id!, Quantity = 1, Price = product.CurrentPrice.Hardcover });
            //     HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);
            // }

            if (HttpContext.Session.GetObjectFromJson<List<ProductListItem>>(GetCartKey()) == null)
            {
                var cart = new List<ProductListItem>();
                var product = _productRepository.GetById(id);

                if (product is null)
                {
                    return NotFound();

                }

                cart.Add(new ProductListItem { ProductDetail = product.Id, Quantity = 1, Price = product.CurrentPrice.Hardcover });
                HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);
            }
            else
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<ProductListItem>>(GetCartKey());
                int index = IsExist(id);
                if (index != -1)
                {
                    if (decs == "decs")
                    {
                        cart[index].Quantity--;
                        HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);
                        return RedirectToAction("Index");
                    }

                    if (incs == "incs")

                    {
                        cart[index].Quantity++;
                        HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);
                        return RedirectToAction("Index", cart);

                    }
                    cart[index].Quantity++;


                }
                else
                {
                    Product product = _productRepository.GetById(id);

                    cart.Add(new ProductListItem { ProductDetail = product.Id, Quantity = 1, Price = product.CurrentPrice.Hardcover });
                }
                HttpContext.Session.SetObjectAsJson(GetCartKey(), cart);
            }
            return RedirectToAction("Index", "Book");
        }

        [HttpPost("cart/remove-item")]
        public IActionResult Remove(string id)
        {
            List<ProductListItem> cart = HttpContext.Session.GetObjectFromJson<List<ProductListItem>>(GetCartKey());
            int index = IsExist(id);
            if (index != -1)
            {
                cart.RemoveAt(index);
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, GetCartKey(), cart);
            return RedirectToAction("Index");
        }


        private int IsExist(string id)
        {
            var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, GetCartKey());
            for (var i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductDetail.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}