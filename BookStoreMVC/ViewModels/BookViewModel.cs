using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.ViewModels;

public class BookViewModel
{
    public string? Id { get; set; }

    public string Title { get; set; } = null!;

    public int PageCount { get; set; }

    public string? Author { get; set; }

    public string? Language { get; set; }

    [DisplayName("Created At")]
    [DataType(DataType.Date)]
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
    [DataType(DataType.Date)]
    [DisplayName("Publish Date")]
    public DateTime PublishDate { get; set; }

    public string? Publisher { get; set; }
    [DisplayName("ISBN")]
    public string? Isbn { get; set; }
    [MaxLength(5000)]
    public string? Description { get; set; }
}