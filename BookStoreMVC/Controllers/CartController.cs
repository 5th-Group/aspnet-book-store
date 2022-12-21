using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStoreMVC.Models.Cart;
using BookStoreMVC.Mapper;


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




        public CartController(ILanguageRepository languageRepository, IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository, IProductRepository productRepository, UserManager<User> userManager, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository)
        {
            _languageRepository = languageRepository;
            _productRepository = productRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _helpersRepository = helpersRepository;
            _userManager = userManager;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
        }

        public string GetCartKey()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public IActionResult Index(int? pageNumber = 1)
        {

            IEnumerable<ProductListItem> cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, GetCartKey());



            if (cart != null && cart.Any())
            {
                foreach (var item in cart)
                {

                    var product = _productRepository.GetById(item.ProductDetail);
                    var book = _bookRepository.GetById(product.BookId).Result;
                    var author = _authorRepository.GetById(product.BookId).Result;
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
                HttpContext.Session.SetString(User.FindFirstValue(ClaimTypes.NameIdentifier), string.Empty);
                // }
            }


            var result = PaginatedList<ShoppingCartItem>.Create(_cartItemList, pageNumber ?? 1, PAGE_SIZE, Headers, "CartIndex");
            return View(result);


            // ViewBag.total = cart.ShoppingCartItems.Sum(item => item.Book.Price * item.Quantity);



            // return View();

        }

        public async Task<IActionResult> AddToCart(string id, string decs, string incs)
        {

            if (SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, GetCartKey()) == null)
            {
                List<ProductListItem> cart = new List<ProductListItem>();
                Product product = _productRepository.GetById(id);

                if (product == null)
                {
                    return NotFound();

                }

                cart.Add(new ProductListItem { ProductDetail = product.Id, Quantity = 1, Price = product.CurrentPrice.Hardcover });
                SessionHelper.SetObjectAsJson(HttpContext.Session, GetCartKey(), cart);

            }
            else
            {
                var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, GetCartKey());
                int index = IsExist(id);
                if (index != -1)
                {
                    if (decs == "decs")
                    {
                        cart[index].Quantity--;
                        SessionHelper.SetObjectAsJson(HttpContext.Session, GetCartKey(), cart);
                        return RedirectToAction("Index");
                    }
                    else if (incs == "incs")

                    {
                        cart[index].Quantity++;
                        SessionHelper.SetObjectAsJson(HttpContext.Session, GetCartKey(), cart);
                        return RedirectToAction("Index", cart);

                    }
                    cart[index].Quantity++;


                }
                else
                {
                    Product product = _productRepository.GetById(id);

                    cart.Add(new ProductListItem { ProductDetail = product.Id, Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, GetCartKey(), cart);
            }
            return RedirectToAction("Index", "Book");
        }

        [HttpPost]
        public IActionResult Remove(string id)
        {
            List<ProductListItem> cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, GetCartKey());
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
            List<ProductListItem> cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, GetCartKey());
            for (int i = 0; i < cart.Count; i++)
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