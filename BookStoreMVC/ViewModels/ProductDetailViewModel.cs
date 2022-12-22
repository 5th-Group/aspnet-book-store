using BookStoreMVC.Models;
using BookStoreMVC.ViewModels.Book;

namespace BookStoreMVC.ViewModels;

public class ProductDetailViewModel
{
    public string? Id { get; set; }

    public DetailBookViewModel Book { get; set; }

    // public decimal Cost { get; set; }
    public PriceStruct Price { get; set; }

    public int Rating { get; set; }

    public IEnumerable<ReviewViewModel> Reviews { get; set; }

}