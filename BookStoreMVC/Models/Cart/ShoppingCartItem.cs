using System.ComponentModel.DataAnnotations;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Models
{
    public class ShoppingCartItem
    {
        public ProductViewModel Product { get; set; }
        public int Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Price { get; set; }
    }
}