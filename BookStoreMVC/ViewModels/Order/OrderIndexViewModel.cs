namespace BookStoreMVC.ViewModels;

public class OrderIndexViewModel
{

    public string? Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string PaymentStatus { get; set; } = null!;
    public int? ShippingStatus { get; set; } = null!;


    public decimal TotalPrice { get; set; }


    public UserDetailViewModel Customer { get; set; } = null!;


}











