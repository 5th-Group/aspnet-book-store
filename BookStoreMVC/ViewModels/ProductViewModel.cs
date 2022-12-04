namespace BookStoreMVC.ViewModels;

public class ProductViewModel
{
    public string? Id { get; set; }

    public BookViewModel Book { get; set; }

    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    
    // public IList<ReviewViewModel>? Reviews { get; set; }
}