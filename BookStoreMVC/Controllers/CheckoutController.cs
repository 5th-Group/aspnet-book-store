using System.Security.Claims;
using System.Text;
using BookStoreMVC.Helpers;
using BookStoreMVC.Mapper;
using BookStoreMVC.Models;
using BookStoreMVC.Models.Payment;
using BookStoreMVC.Models.Cart;
using BookStoreMVC.Services;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
        private readonly UserManager<User> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHelpers _helpersRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly ILanguageRepository _languageRepository;

        public CheckoutController(IPaymentStrategy paymentStrategy, IConfiguration configuration, SignInManager<User> signInManager, IOrderRepository orderRepository, IBookRepository bookRepository, IAuthorRepository authorRepository, IHelpers helpersRepository, IProductRepository productRepository, IBookGenreRepository bookGenreRepository, IPublisherRepository publisherRepository, ILanguageRepository languageRepository, UserManager<User> userManager)
        {
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
            _userManager = userManager;
            StripeConfiguration.ApiKey = configuration.GetValue<string>(
                "Stripe:SecretKey");
        }

        public string GetCartKey()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "GHOST_USR";
        }

        [HttpGet("/summary")]
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
        [Authorize("RequireAuthenticated")]
        [HttpGet("pay")]
        public IActionResult Pay()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, userId);


            MomoPaymentRequest momo = new MomoPaymentRequest
            {
                requestId = DateTime.Now.Ticks.ToString(),
                amount = cart.Select(item => Convert.ToInt64(item.Price * item.Quantity)).Sum() + 15_000,
                orderId = ObjectId.GenerateNewId().ToString()
            };
            _configuration.GetSection("PaymentSettings:Momo").Bind(momo);
            momo.orderInfo += userId;
            // momo.redirectUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + momo.redirectUrl;
            // momo.ipnUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + momo.ipnUrl;
            momo.extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cart)));



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


        [Authorize("RequireAuthenticated")]
        [HttpGet("stripe")]

        public IActionResult Stripe()
        {
            return View();
        }

        
        [HttpPost("create-payment-intent")] // https://swiftlib.site/checkout/create-payment-intent
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

        [HttpGet("stripe/success")]
        [Authorize("RequireAuthenticated")]
        public async Task<IActionResult> StripSuccess([FromQuery] string payment_intent)
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

                return View("Success");
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;

                return View("Success");
            }
        }


        [HttpGet("stripe/failure")]
        [Authorize("RequireAuthenticated")]
        public async Task<IActionResult> StripeFailed([FromQuery] string paymentIntentId)
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

                return View("Failure");
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;

                return View("Failure");
            }
        }
        #endregion


        [HttpGet("momo-redirect")]
        public async Task<IActionResult> MomoResult([FromQuery] MomoNotification momoNotification)
        {
            if (momoNotification.resultCode != 0) return RedirectToAction("MomoFailure");

            var user = _userManager.FindByIdAsync(momoNotification.orderInfo.Split("#")[1]).Result;

            var order = new Order
            {
                Id = momoNotification.orderId,
                ProductList =
                    JsonConvert.DeserializeObject<IList<ProductListItem>>(
                        Encoding.UTF8.GetString(Convert.FromBase64String(momoNotification.extraData)))!,
                CreatedAt = default,
                PaymentStatus = "Paid",
                ShippingStatus = new[]
                {
                    new OrderStatus
                    {
                        Name = "Order has been confirmed",
                        TimeStamp = DateTime.Now
                    }
                },
                CurrentShippingStatus = 0,
                ShippingAddress = user.Address.First().Location,
                Customer = user.Id.ToString(),
                TotalPrice = momoNotification.amount
            };

             await _orderRepository.AddAsync(order);

             return RedirectToAction("MomoSuccess");


            // return RedirectToAction(momoNotification.resultCode == 0 ? "MomoSuccess" : "MomoFailure");
        }

        [HttpPost("momo-ipn")]
        public IActionResult MomoIpn([FromBody] MomoNotification? momoNotification)
        {
            if (momoNotification is null || momoNotification.resultCode != 0) return NoContent();

            
            if (_orderRepository.GetByOrderId(momoNotification.orderId).Result is not null) return NoContent();
            
            var model = Encoding.UTF8.GetString(Convert.FromBase64String(momoNotification.extraData));
            var items = JsonConvert.DeserializeObject<IList<ProductListItem>>(model);
            
            var user = _userManager.FindByIdAsync(momoNotification.orderInfo.Split("#")[1]).Result;

            var order = new Order
            {
                Id = momoNotification.orderId,
                ProductList = items!,
                PaymentStatus = "Paid",
                ShippingStatus = new[]
                {
                    new OrderStatus
                    {
                        Name = "Order has been confirmed",
                        TimeStamp = DateTime.Now
                    }
                },
                CurrentShippingStatus = 0,
                ShippingAddress = user.Address.First().Location,
                Customer = momoNotification.orderInfo.Split("#")[1],
                TotalPrice = momoNotification.amount
            };

            _orderRepository.AddAsync(order);

            return NoContent();
        }

        [HttpGet("momo/success")]
        public IActionResult MomoSuccess()
        {
            return View("Success");
        }

        [HttpGet("momo/failure")]
        public IActionResult MomoFailure()
        {
            return View("Failure");
        }
    }
}