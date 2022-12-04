namespace BookStoreMVC.ViewModels;

public class CartViewModel
{
    public string? Id { get; set; }

    public IEnumerable<CartDetailViewModel> Items { get; set; }

    public int Count => Items.Count();
    
    public decimal TotalSum => Items.Select(item => item.Product.Price).Sum();
}