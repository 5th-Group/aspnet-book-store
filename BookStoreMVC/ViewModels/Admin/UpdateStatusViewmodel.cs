namespace BookStoreMVC.Models.Payment;

public class UpdateStatusViewmModel
{
    public string? orderId { get; set; }
    public string Name { get; set; } = null!;

    public DateTime TimeStamp { get; set; } = DateTime.Now;
}