using System.Security.Claims;
using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using BookStoreMVC.Controllers;

namespace BookStoreMVC.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;

        public PaymentsController(IConfiguration configuration, IOrderRepository orderRepository, UserManager<User> userManager)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration.GetValue<string>(
                "Stripe:SecretKey");
        }

        public string GetCartKey()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public IActionResult Payment()
        {
            return View();
        }
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

                var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, userId);

                var order = new Order
                {
                    Customer = userId,
                    ProductList = cart,
                    TotalPrice = Convert.ToDecimal(TotalPriceCalc(cart))
                };


                if (result.Status == "succeeded")
                {
                    order.Status = "Paid";
                    await _orderRepository.AddAsync(order);
                    ViewBag.Result = "Payment succeeded!";
                    // To do : 
                    // - Store ORDER to database 
                    // - Clear cart (session)
                }
                else
                {
                    order.Status = "Payment failed";
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




        public async Task<IActionResult> Failed([FromQuery] string paymentIntent_id)
        {
            if (string.IsNullOrEmpty(paymentIntent_id))
            {
                ViewBag.Result = "Nothing here, go back!";
                return View();
            }


            try
            {
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent result = await paymentIntentService.GetAsync(paymentIntent_id);

                string userId = GetCartKey();

                var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, userId);

                var order = new Order
                {
                    Customer = userId,
                    ProductList = cart,
                    Status = "Failed"
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
