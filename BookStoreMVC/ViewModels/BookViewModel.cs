using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.ViewModels;

public class BookViewModel
{
    public string? Id { get; set; }

    public string Title { get; set; } = null!;

    public int PageCount { get; set; }
    
    public string? Author { get; set; }
    
    public string? Language { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public IList<string> Genre { get; set; } = null!;

    public IList<string> Type { get; set; } = null!;

    public List<SelectListItem> TypeList { get; } = new()
    {
        new SelectListItem("Paperback", "Paperback"),
        new SelectListItem("Hardcover", "Hardcover"),
        new SelectListItem("E-book", "E-book")
    };

    public IFormFile? Img { get; set; }

    public string? ImageUri { get; set; }

    public string? ImageName { get; set; }

    public string? SignedUrl { get; set; }

    public DateTime PublishDate { get; set; }
    
    public string? Publisher { get; set; }

    public string? Isbn { get; set; }

    public string? Description { get; set; }
}