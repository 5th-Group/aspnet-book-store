using BookStoreMVC.Services;

namespace BookStoreMVC.Models;

public class MomoPaymentRequest : IPaymentModel
{
    public string? partnerCode { get; set; }

    public string? requestId { get; set; }

    public long amount { get; set; }

    public string? orderId { get; set; }

    public string? orderInfo { get; set; }

    public string? redirectUrl { get; set; }

    public string? ipnUrl { get; set; }

    public string? requestType { get; set; }

    public string? extraData { get; set; }

    public string? lang { get; set; }

    public string? signature { get; set; }
}