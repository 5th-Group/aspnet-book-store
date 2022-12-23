using System.Security.Claims;
using System.Text;
using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Models.Payment;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BookStoreMVC.Controllers
{
    [Route("checkout")]
    public class CheckoutController : Controller
    {
        private readonly IPaymentStrategy _paymentStrategy;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly IOrderRepository _orderRepository;

        public CheckoutController(IPaymentStrategy paymentStrategy, IConfiguration configuration, SignInManager<User> signInManager, IOrderRepository orderRepository)
        {
            _paymentStrategy = paymentStrategy;
            _configuration = configuration;
            _signInManager = signInManager;
            _orderRepository = orderRepository;
        }


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