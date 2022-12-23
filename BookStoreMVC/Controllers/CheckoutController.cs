using System.Security.Claims;
using System.Text;
using BookStoreMVC.Helpers;
using BookStoreMVC.Mapper;
using BookStoreMVC.Models;
using BookStoreMVC.Models.Cart;
using BookStoreMVC.Models.Payment;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using IronBarCode;
using IronSoftware.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Stripe;

namespace BookStoreMVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IPaymentStrategy _paymentStrategy;
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;

        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHelpers _helpersRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly ILanguageRepository _languageRepository;


        public CheckoutController(IPaymentStrategy paymentStrategy, IConfiguration configuration, IOrderRepository orderRepository, ILanguageRepository languageRepository, IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository, IProductRepository productRepository, UserManager<User> userManager, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository)
        {
            _paymentStrategy = paymentStrategy;
            _configuration = configuration;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _languageRepository = languageRepository;
            _productRepository = productRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _helpersRepository = helpersRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
            StripeConfiguration.ApiKey = configuration.GetValue<string>(
               "Stripe:SecretKey");
        }

        public string GetCartKey()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "GHOST_USR";
        }


        public IActionResult Index()
        {
            var key = GetCartKey();
            if (key != "GHOST_USR" && HttpContext.Session.Keys.Contains("GHOST_USR"))
            {
                HttpContext.Session.SetObjectAsJson(key,
                    HttpContext.Session.GetObjectFromJson<List<ProductListItem>>("GHOST_USR"));
                HttpContext.Session.Remove("GHOST_USR");
            }

            var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, key);

            IList<ShoppingCartItem> _cartItemList = new List<ShoppingCartItem>();

            if (cart != null && cart.Any())
            {
                foreach (var item in cart)
                {

                    var product = _productRepository.GetById(item.ProductDetail);
                    var book = _bookRepository.GetById(product.BookId).Result;
                    var author = _authorRepository.GetById(book.Author).Result;
                    var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre));
                    var publisher = _publisherRepository.GetById(book.Publisher);
                    var language = _languageRepository.GetByIdAsync(book.Language).Result;

                    var bookViewModel = MapBook.MapIndexBookViewModel(book, author, bookGenres, publisher, language, _helpersRepository);



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
                return NotFound();
                // Generate Empty cart list so user can still access cart page instead of throwing error
                // HttpContext.Session.SetString(User.FindFirstValue(ClaimTypes.NameIdentifier), string.Empty); 
                // }
            }
            return View(_cartItemList);
        }

        #region Momo
        [Authorize("RequireUserRole")]
        [HttpGet("checkout/pay")]
        public IActionResult Pay()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, userId);


            MomoPaymentRequest momo = new MomoPaymentRequest
            {
                requestId = DateTime.Now.Ticks.ToString(),
                amount = cart.Select(item => Convert.ToInt64((item.Price * item.Quantity))).Sum(),
                orderId = DateTime.Now.Ticks.ToString(),
            };
            _configuration.GetSection("PaymentSettings:Momo").Bind(momo);
            momo.orderInfo += momo.orderId;
            momo.redirectUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + momo.redirectUrl;
            momo.ipnUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + momo.ipnUrl;
            momo.extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(cart.ToString()));



            var result = _paymentStrategy.MakePayment(momo);
            var jRes = string.IsNullOrEmpty(result) ? null : JObject.Parse(result);
            if (jRes != null && jRes.Value<int>("resultCode") == 0)
            {
                // GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(jRes.GetValue("qrCodeUrl")!.ToString());
                // barcode.ChangeBarCodeColor(Color.Cyan);
                // ViewData["qrCode"] = barcode.ToDataUrl();

                return Redirect(jRes.GetValue("payUrl")!.ToString());
            }

            // return Redirect(jRes.GetValue("payUrl")!.ToString());


            return RedirectToAction("Index", "Home");
        }



        #endregion

        #region Stripe




        private int TotalPriceCalc(IList<ProductListItem> list)
        {
            decimal toltal = 0;
            foreach (var item in list)
            {
                toltal += item.TotalPrice;
            }

            return Convert.ToInt32(toltal);
        }


        [Authorize("RequireUserRole")]
        [HttpGet("checkout/stripe")]

        public IActionResult Stripe()
        {
            return View();
        }


        [Authorize("RequireUserRole")]
        [HttpPost("create-payment-intent")]
        public ActionResult Create()
        {
            var key = GetCartKey();
            if (key != "GHOST_USR" && HttpContext.Session.Keys.Contains("GHOST_USR"))
            {
                HttpContext.Session.SetObjectAsJson(key,
                    HttpContext.Session.GetObjectFromJson<List<ProductListItem>>("GHOST_USR"));
                HttpContext.Session.Remove("GHOST_USR");
            }

            IEnumerable<ProductListItem> cart = HttpContext.Session.GetObjectFromJson<List<ProductListItem>>(key);


            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = TotalPriceCalc(cart.ToList()),
                Currency = "vnd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });

            return Json(new { clientSecret = paymentIntent.ClientSecret, paymentIntent_id = paymentIntent.Id });
        }

        [HttpGet]
        [Authorize("RequireUserRole")]
        public async Task<IActionResult> Success([FromQuery] string payment_intent)
        {
            if (string.IsNullOrEmpty(payment_intent))
            {
                return NotFound();
            }


            try
            {
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent result = await paymentIntentService.GetAsync(payment_intent);

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cart = HttpContext.Session.GetObjectFromJson<List<ProductListItem>>(userId);

                var order = new Order
                {
                    Customer = userId,
                    ProductList = cart,
                    TotalPrice = Convert.ToDecimal(TotalPriceCalc(cart))
                };


                if (result.Status == "succeeded")
                {
                    order.PaymentStatus = "Paid";
                    order.ShippingStatus = new[]
                    {
                        new OrderStatus
                        {
                            Name = "Order has been confirmed",
                            TimeStamp = DateTime.Now
                        }
                    };
                    order.CurrentShippingStatus = 0;

                    await _orderRepository.AddAsync(order);
                    ViewBag.Result = "Payment succeeded!";
                    HttpContext.Session.Remove(userId);

                }
                else
                {
                    order.PaymentStatus = "Payment failed";
                    await _orderRepository.AddAsync(order);
                    ViewBag.Result = "Payment failed!";
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;

                return View();
            }
        }


        public async Task<IActionResult> Failed([FromQuery] string paymentIntentId)
        {
            if (string.IsNullOrEmpty(paymentIntentId))
            {
                return NotFound();
            }


            try
            {
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent result = await paymentIntentService.GetAsync(paymentIntentId);

                string userId = GetCartKey();

                var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, userId);

                var order = new Order
                {
                    Customer = userId,
                    ProductList = cart,
                    PaymentStatus = "Failed"
                };

                await _orderRepository.AddAsync(order);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;

                return View();
            }
        }
        #endregion



    }
}