using BookStoreMVC.Models;

namespace BookStoreMVC.ViewModels;

public class PublisherViewModel
{
    public string? Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public Contact Contact { get; set; } = null!;
    
    public string Origin { get; set; } = null!;
}