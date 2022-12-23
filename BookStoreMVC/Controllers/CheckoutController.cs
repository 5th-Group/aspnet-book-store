using System.Security.Claims;
using System.Text;
using BookStoreMVC.Helpers;
using BookStoreMVC.Mapper;
using BookStoreMVC.Models;
using BookStoreMVC.Models.Payment;
using BookStoreMVC.Models.Cart;
using BookStoreMVC.Models.Payment;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;

namespace BookStoreMVC.Controllers
{
    [Route("checkout")]
    public class CheckoutController : Controller
    {
        private readonly IPaymentStrategy _paymentStrategy;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHelpers _helpersRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly ILanguageRepository _languageRepository;

        public CheckoutController(IPaymentStrategy paymentStrategy, IConfiguration configuration, SignInManager<User> signInManager, IOrderRepository orderRepository, IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository, IProductRepository productRepository, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository, ILanguageRepository languageRepository){
        _paymentStrategy = paymentStrategy;
            _configuration = configuration;
            _signInManager = signInManager;
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _helpersRepository = helpersRepository;
            _productRepository = productRepository;
            _bookGenreRepository = bookGenreRepository;
            _publisherRepository = publisherRepository;
            _languageRepository = languageRepository;
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
            var cart = HttpContext.Session.GetObjectFromJson<List<ProductListItem>>(key);

            IList<ShoppingCartItem> cartItemList = new List<ShoppingCartItem>();

            var projectionDef = Builders<Book>.Projection
                .Exclude(d => d.PageCount)
                .Exclude(d => d.CreatedAt)
                .Exclude(d => d.ImageUri)
                .Exclude(d => d.PublishDate)
                .Exclude(d => d.Description);

            if (cart != null && cart.Any())
            {
                foreach (var item in cart)
                {
                    var taskList = new List<Task>();
                    
                    var product = _productRepository.GetById(item.ProductDetail);
                    
                    var book = _bookRepository.GetById(product.BookId, projectionDef).Result;

                    var author =
                        _authorRepository.GetWithFilterAsync(
                            Builders<Author>.Filter.Where(a => a.Id == book.Author));
                    
                    taskList.Add(author);

                    var publisher =
                        _publisherRepository.GetWithFilterAsync(
                            Builders<Publisher>.Filter.Where(p => p.Id == book.Publisher));
                    taskList.Add(publisher);

                    var lang = _languageRepository.GetWithFilterAsync(
                        Builders<Language>.Filter.Where(l => l.Id == book.Language));
                    taskList.Add(lang);
                    
                    var bookGenres = book.Genre.Select(genre => _bookGenreRepository.GetById(genre).Result);

                    var t1 = Task.WhenAll(taskList);
                    t1.Wait();


                    var bookViewModel = MapBook.MapCartBookViewModel(book, author.Result, bookGenres, publisher.Result, lang.Result, _helpersRepository);

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


                    cartItemList.Add(cartItem);
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
            return View(cartItemList);
        }

        #region Momo
        [Authorize("RequireUserRole")]
        [HttpGet("pay")]
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
            momo.orderInfo += userId;
            // momo.redirectUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + momo.redirectUrl;
            // momo.ipnUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + momo.ipnUrl;
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

        [HttpGet("")]
        public IActionResult Payment()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Unauthorized();
            }
            
            return View();
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


        [HttpGet("momo-redirect")]
        public IActionResult MomoResult([FromQuery] MomoNotification momoNotification)
        {
            return RedirectToAction(momoNotification.resultCode == 0 ? "Success" : "Failure");
        }
        
        [HttpPost("momo-ipn")]
        public IActionResult MomoIpn([FromBody]MomoNotification? momoNotification)
        {
            if (momoNotification is null || momoNotification.resultCode != 0) return NoContent();

            var model = Encoding.UTF8.GetString(Convert.FromBase64String(momoNotification.extraData));
            var items = JsonConvert.DeserializeObject<IList<ProductListItem>>(model);

            var order = new Order
            {
                ProductList = items!,
                PaymentStatus = "Paid",
                ShippingStatus = new []
                {
                    new OrderStatus
                    {
                        Name = "Order has been confirmed",
                        TimeStamp = DateTime.Now
                    }
                },
                CurrentShippingStatus = 0,
                Customer = momoNotification.orderInfo.Split("#")[1],
                TotalPrice = momoNotification.amount
            };

            _orderRepository.AddAsync(order);

            return NoContent();
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return View();
        }
        
        [HttpGet("failure")]
        public IActionResult Failure()
        {
            return View();
        }
    }
}