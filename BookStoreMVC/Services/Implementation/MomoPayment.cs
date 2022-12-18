using System.Security.Cryptography;
using System.Text;
using BookStoreMVC.Models;
using Newtonsoft.Json;

namespace BookStoreMVC.Services.Implementation;

public class MomoPayment : PaymentService<MomoPaymentRequest>
{
    private readonly IHttpClientFactory _clientFactory;

    // private MomoPaymentRequest _request = new();
    private readonly string _accessKey, _secretKey; 

    public MomoPayment(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        
        // configuration.GetSection("PaymentSettings:Momo").Bind(_request);

        _accessKey = configuration["PaymentSettings:Momo:AccessKey"];
        _secretKey = configuration["PaymentSettings:Momo:SecretKey"];
    }

    protected override string MakePayment(MomoPaymentRequest model)
    {
        using var client = _clientFactory.CreateClient("momo-payment");

        var rawHash = GenerateRawHash(model);
        
        // JObject.FromObject(model).Property("extraData")!.Remove();

        model.signature = Convert.ToHexString(Sign_SHA256(rawHash)).ToLower();

        var body = new StringContent(GenerateRequestBody(model), Encoding.UTF8, "application/json");

        var response = client.PostAsync(client.BaseAddress, body).Result;

        // client.PostAsync()
        return response.Content.ReadAsStringAsync().Result ;
    }

    private string GenerateRawHash(MomoPaymentRequest model)
    {
        return
            $"accessKey={_accessKey}&amount={model.amount}&extraData={model.extraData}&ipnUrl={model.ipnUrl}&orderId={model.orderId}&orderInfo={model.orderInfo}&partnerCode={model.partnerCode}&redirectUrl={model.redirectUrl}&requestId={model.requestId}&requestType={model.requestType}";
    }

    private ReadOnlySpan<byte> Sign_SHA256(string message)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        ReadOnlySpan<byte> stringBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return stringBytes;
    }

    private static string GenerateRequestBody(MomoPaymentRequest model)
    {
        return JsonConvert.SerializeObject(model);
    }
}