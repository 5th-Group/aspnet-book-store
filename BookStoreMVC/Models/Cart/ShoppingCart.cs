using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public IList<ShoppingCartItem> ShoppingCartItems { get; set; }

    }
}