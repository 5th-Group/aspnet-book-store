using BookStoreMVC.Models;
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
            var momo = new MomoPaymentRequest();
            _configuration.GetSection("PaymentSettings:Momo").Bind(momo);
            
            _paymentStrategy.MakePayment(momo);
            return null;
        }

        public IActionResult Index() => View();
    }
}