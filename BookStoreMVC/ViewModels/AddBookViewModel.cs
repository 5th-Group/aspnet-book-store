using System.ComponentModel.DataAnnotations;
using BookStoreMVC.Models;

namespace BookStoreMVC.ViewModels;

public class AddBookViewModel
{
    public string? Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int PageCount { get; set; }
    
    public IEnumerable<string> Type { get; set; }

    public string Author { get; set; }

    public string Language { get; set; }

    public IEnumerable<string> Genre { get; set; }

    public string Publisher { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime PublishDate { get; set; }

    public string? ImageUri { get; set; }

    public string? ImageName { get; set; }

    public string? SignedUrl { get; set; }
}