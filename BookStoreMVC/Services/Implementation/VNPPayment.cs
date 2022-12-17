using System.Security.Cryptography;
using System.Text;
using BookStoreMVC.Models;
using Newtonsoft.Json;

namespace BookStoreMVC.Services.Implementation;

// ReSharper disable once InconsistentNaming
public class VNPPayment : PaymentService<VNPPaymentRequest>
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _secretKey;

    public VNPPayment(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;

        _secretKey = configuration["PaymentSettings:VNPay:SecretKey"];
    }

    protected override string MakePayment(VNPPaymentRequest model)
    {
        using var client = _clientFactory.CreateClient("vnp-payment");

        var rawHash = GenerateRawHash(model);

        model.vnp_SecureHash = Convert.ToHexString(Sign_SHA512(rawHash)).ToLower();

        var body = new StringContent(GenerateRequestBody(model), Encoding.UTF8, "application/json");

        var response = client.PostAsync(client.BaseAddress, body).Result;

        return response.Content.ReadAsStringAsync().Result;
    }

    private string GenerateRawHash(VNPPaymentRequest model)
    {
        return
            $"vnp_Amount={model.vnp_Amount}&vnp_Command={model.vnp_Command}&vnp_CreateDate={model.vnp_CreateDate}&vnp_CurrCode={model.vnp_CurrCode}&vnp_IpAddr={model.vnp_IpAddr}&vnp_Locale={model.vnp_Locale}&vnp_OrderInfo={model.vnp_OrderInfo}&vnp_ReturnUrl={model.vnp_ReturnUrl}&vnp_TmnCode={model.vnp_TmnCode}&vnp_TxnRef={model.vnp_TxnRef}&vnp_Version={model.vnp_Version}";
    }

    private ReadOnlySpan<byte> Sign_SHA512(string message)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secretKey));
        ReadOnlySpan<byte> stringBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return stringBytes;
    }

    private static string GenerateRequestBody(VNPPaymentRequest model)
    {
        return JsonConvert.SerializeObject(model);
    }
}