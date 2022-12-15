using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookStoreMVC.Controllers
{

    public class CartController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHelpers _helpersRepository;

        private IList<ShoppingCartItem> cartItemList = new List<ShoppingCartItem>();

        private const int PAGE_SIZE = 10;

        private IEnumerable<string>? Headers;


        public const string CARTKEY = "cart";
        public CartController(IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _helpersRepository = helpersRepository;
        }
        public IActionResult Index(int? pageNumber = 1)
        {
            IEnumerable<ProductListItem> cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, CARTKEY);
            // var shoppingCart = new ShoppingCart
            // {
            //     ShoppingCartItems = cart
            // };

            if (cart != null)
            {
                foreach (var item in cart)
                {
                    var book = _bookRepository.GetById(item.ProductDetail).Result;
                    IndexBookViewModel bookVm = new IndexBookViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        AuthorDisplay = _authorRepository.GetById(book.Author).Result,
                        SignedUrl = _helpersRepository.GenerateSignedUrl(book.ImageName).Result,
                    };

                    var cartItem = new ShoppingCartItem
                    {
                        Book = bookVm,
                        Amount = item.Quantity,

                    };
                    cartItemList.Add(cartItem);
                }

            }

            var result = PaginatedList<ShoppingCartItem>.Create(cartItemList, pageNumber ?? 1, PAGE_SIZE, Headers, "CartIndex");
            return View(result);


            // ViewBag.total = cart.ShoppingCartItems.Sum(item => item.Book.Price * item.Quantity);



            // return View();

        }

        public async Task<IActionResult> AddToCart(string id, string decs, string incs)
        {

            if (SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, CARTKEY) == null)
            {
                List<ProductListItem> cart = new List<ProductListItem>();
                Book b = _bookRepository.GetById(id).Result;



                if (b == null)
                {
                    return NotFound();

                }
                cart.Add(new ProductListItem { ProductDetail = b.Id, Quantity = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);

            }
            else
            {
                var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, CARTKEY);
                int index = isExist(id);
                if (index != -1)
                {
                    if (decs == "decs")
                    {
                        cart[index].Quantity--;
                        SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);
                        return RedirectToAction("Index");
                    }
                    else if (incs == "incs")

                    {
                        cart[index].Quantity++;
                        SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);
                        return RedirectToAction("Index", cart);

                    }
                    cart[index].Quantity++;


                }
                else
                {
                    Book b = await _bookRepository.GetById(id);

                    cart.Add(new ProductListItem { ProductDetail = b.Id, Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, CARTKEY, cart);
            }
            return RedirectToAction("Index", "Book");
        }

        [HttpPost]
        public IActionResult Remove(string id)
        {
            List<ProductListItem> cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, CARTKEY);
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
            List<ProductListItem> cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, CARTKEY);
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