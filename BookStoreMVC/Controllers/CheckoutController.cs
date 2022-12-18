using System.Security.Claims;
using System.Text;
using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BookStoreMVC.Controllers
{
    public class CheckoutController : Controller
    {
        // private readonly IPaymentService _momoPayment;
        // private readonly IPaymentService _vnpPayment;
        private readonly IPaymentStrategy _paymentStrategy;
        private readonly IConfiguration _configuration;

        public CheckoutController(IPaymentStrategy paymentStrategy, IConfiguration configuration)
        {
            _paymentStrategy = paymentStrategy;
            _configuration = configuration;
            // _vnpPayment = serviceResolver(PaymentServiceEnum.VNPPayment);
            // _momoPayment = serviceResolver(PaymentServiceEnum.MomoPayment);
        }

        
        [Authorize("RequireUserRole")]
        [HttpPost]
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
            momo.ipnUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + momo.redirectUrl;
            momo.extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(cart.ToString()));

            var result = _paymentStrategy.MakePayment(momo);
            var jRes = JObject.Parse(result);
            if (jRes != null && jRes.Value<int>("resultCode") == 0) return Redirect(jRes.GetValue("payUrl")!.ToString());

            return null;
        }

        public IActionResult Index() => View();
        
        
        
    }
}