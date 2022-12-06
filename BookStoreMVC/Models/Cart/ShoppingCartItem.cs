using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public IndexBookViewModel? Book { get; set; }
        public int Amount { get; set; }
    }
}