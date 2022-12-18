using BookStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

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


        public IActionResult Pay()
        {
            // var momo = new MomoPaymentRequest
            // {
            //     partnerCode = null,
            //     requestId = null,
            //     amount = 0,
            //     orderId = null,
            //     orderInfo = null,
            //     redirectUrl = null,
            //     ipnUrl = null,
            //     requestType = null,
            //     extraData = null,
            //     lang = null,
            //     signature = null
            // };
            // var momo = new MomoPaymentRequest();
            // _configuration.GetSection("PaymentSettings:Momo").Bind(momo);
            //
            // var res = _paymentStrategy.MakePayment(momo);
            // var jobject = JObject.Parse(res);
            // var qrGenerator = new QRCodeGenerator();
            // var qrData = qrGenerator.CreateQrCode(jobject.GetValue("qrCodeUrl")!.ToString(), QRCodeGenerator.ECCLevel.Q);
            // // var qrCode = new QRCode(qrData);

            return null;
        }

        public IActionResult Index() => View();
    }
}