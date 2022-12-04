namespace BookStoreMVC.ViewModels;

public class ProductViewModel
{
    public string? Id { get; set; }

    public IndexBookViewModel Book { get; set; }

    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    
}