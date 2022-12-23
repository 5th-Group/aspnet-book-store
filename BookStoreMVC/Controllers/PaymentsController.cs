using System.Security.Claims;
using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Models.Payment;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BookStoreMVC.Controllers
{
    public class PaymentsController : Controller
    {
        // private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;

        public PaymentsController(IConfiguration configuration, IOrderRepository orderRepository,
            UserManager<User> userManager)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            // _configuration = configuration;
            StripeConfiguration.ApiKey = configuration.GetValue<string>(
                "Stripe:SecretKey");
        }

        public string GetCartKey()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // public IActionResult Payment()
        // {
        //     return View();
        // }
        public IActionResult CheckOut()
        {
            return View();
        }

        private int TotalPriceCalc(IList<ProductListItem> list)
        {
            decimal toltal = 0;
            foreach (var item in list)
            {
                toltal += item.TotalPrice;
            }

            return Convert.ToInt32(toltal);
        }

        [HttpPost("create-payment-intent")]
        public ActionResult Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, userId);


            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = TotalPriceCalc(cart),
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
        public async Task<IActionResult> Success([FromQuery] string paymentIntent)
        {
            if (string.IsNullOrEmpty(paymentIntent))
            {
                return NotFound();
            }
            
            try
            {
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent result = await paymentIntentService.GetAsync(paymentIntent);

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
                    // To do : 
                    // - Store ORDER to database 
                    // - Clear cart (session)
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
                ViewBag.Result = "Nothing here, go back!";
                return View();
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
    }
}