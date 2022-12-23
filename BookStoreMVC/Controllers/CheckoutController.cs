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
using Stripe;


namespace BookStoreMVC.Controllers;

[Route("checkout")]
public class CheckoutController : Controller
{
    private readonly IPaymentStrategy _paymentStrategy;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<User> _signInManager;
    private readonly IOrderRepository _orderRepository;

    public CheckoutController(IPaymentStrategy paymentStrategy, IConfiguration configuration, SignInManager<User> signInManager, IOrderRepository orderRepository){
        _paymentStrategy = paymentStrategy;
        _configuration = configuration;
        _signInManager = signInManager;
        _orderRepository = orderRepository;
        StripeConfiguration.ApiKey = configuration.GetValue<string>(
            "Stripe:SecretKey");
    }

    private string GetCartKey()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "GHOST_USR";
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
        if (!_signInManager.IsSignedIn(User)) return Unauthorized();
        
        // string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // var cart = SessionHelper.GetObjectFromJson<List<ProductListItem>>(HttpContext.Session, userId);


        return View();
    }


    #endregion

    #region Stripe
        
    private static int TotalPriceCalc(IEnumerable<ProductListItem> list)
    {
        return Convert.ToInt32(list.Sum(item => item.TotalPrice));
    }


    [Authorize("RequireUserRole")]
    [HttpGet("stripe")]

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

    [HttpGet("success")]
    [Authorize("RequireUserRole")]
    public async Task<IActionResult> Success([FromQuery]string? paymentIntent)
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
                ShippingAddress = null,
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

    // [HttpGet("success")]
    // public IActionResult Success()
    // {
    //     return View();
    // }
    //
    // [HttpGet("failure")]
    // public IActionResult Failure()
    // {
    //     return View();
    // }
}