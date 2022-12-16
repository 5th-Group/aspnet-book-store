using BookStoreMVC.Services;

namespace BookStoreMVC.Models;

// ReSharper disable once InconsistentNaming
public class VNPPaymentRequest : IPaymentModel
{
    public string? vnp_Version { get; set; }
    
    public string? vnp_Command { get; set; }
    
    public string? vnp_TmnCode { get; set; }
    
    public decimal vnp_Amount { get; set; }
    
    public ulong vnp_CreateDate { get; set; }
    
    public string? vnp_CurrCode { get; set; }
    
    public string? vnp_IpAddr { get; set; }
    
    public string? vnp_Locale { get; set; }
    
    public string? vnp_OrderInfo { get; set; }
    
    public string? vnp_ReturnUrl { get; set; }
    
    public string? vnp_TxnRef { get; set; }
    
    public string? vnp_SecureHash { get; set; }
    
}