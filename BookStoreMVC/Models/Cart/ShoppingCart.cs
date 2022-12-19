namespace BookStoreMVC.Models.Cart
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        
        public IList<ShoppingCartItem> ShoppingCartItems { get; set; }

    }
}