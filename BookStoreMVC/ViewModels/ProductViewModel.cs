using BookStoreMVC.Models;

namespace BookStoreMVC.ViewModels;

public class ProductViewModel
{
    public string? Id { get; set; }


    public IndexBookViewModel Book { get; set; }

    // public decimal Cost { get; set; }
    public PriceStruct Price { get; set; }

    public float Rating { get; set; }

}