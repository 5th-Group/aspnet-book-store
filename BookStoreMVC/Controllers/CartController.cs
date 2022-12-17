using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;




namespace BookStoreMVC.Controllers
{

    public class CartController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHelpers _helpersRepository;
        private readonly IProductRepository _productRepository;

        private readonly UserManager<User> _userManager;

        private IList<ShoppingCartItem> cartItemList = new List<ShoppingCartItem>();

        private const int PAGE_SIZE = 10;

        private IEnumerable<string>? Headers;




        public CartController(IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository, IProductRepository productRepository, UserManager<User> userManager)
        {

            _productRepository = productRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _helpersRepository = helpersRepository;
            _userManager = userManager;



        }

        public string GetCartKey()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public IActionResult Index(int? pageNumber = 1)
        {

            IEnumerable<ProductListItem> cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, GetCartKey());



            if (cart == null)
            {
                return NotFound();

            }

            foreach (var item in cart)
            {

                var product = _productRepository.GetById(item.ProductDetail);
                var book = _bookRepository.GetById(product.BookId).Result;

                var bookVM = new IndexBookViewModel
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
                };



                ProductViewModel productVM = new ProductViewModel
                {
                    Id = product.Id,
                    Price = product.CurrentPrice,
                    Rating = product.AverageScore,
                    Book = bookVM
                };

                var cartItem = new ShoppingCartItem
                {
                    Product = productVM,
                    Amount = item.Quantity,
                    Price = item.TotalPrice
                };
                cartItemList.Add(cartItem);
            }

            var result = PaginatedList<ShoppingCartItem>.Create(cartItemList, pageNumber ?? 1, PAGE_SIZE, Headers, "CartIndex");
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