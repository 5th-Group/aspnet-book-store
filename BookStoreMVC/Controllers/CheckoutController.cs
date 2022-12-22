using System.Security.Claims;
using System.Text;
using BookStoreMVC.Helpers;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using IronBarCode;
using IronSoftware.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BookStoreMVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IPaymentStrategy _paymentStrategy;
        private readonly IConfiguration _configuration;

        public CheckoutController(IPaymentStrategy paymentStrategy, IConfiguration configuration)
        {
            _paymentStrategy = paymentStrategy;
            _configuration = configuration;
        }

        
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

                return Redirect(jRes.GetValue("qrCodeUrl")!.ToString());
            }
                
                // return Redirect(jRes.GetValue("payUrl")!.ToString());


                return RedirectToAction("Index", "Home");
        }

        public IActionResult Index() => View();
        
        
        
    }
}