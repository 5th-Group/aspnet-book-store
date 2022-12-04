using System.ComponentModel.DataAnnotations;
using BookStoreMVC.Models;

namespace BookStoreMVC.ViewModels;

public class PublisherViewModel
{
    public string? Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public Contact Contact { get; set; } = null!;
    [Required]
    public string Origin { get; set; } = null!;
}

