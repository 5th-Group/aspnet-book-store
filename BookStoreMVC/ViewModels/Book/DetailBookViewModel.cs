using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.ViewModels.Book;

public class DetailBookViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int PageCount { get; set; }

    public AuthorViewModel Author { get; set; } = null!;

    public string Language { get; set; } = null!;

    public IEnumerable<BookGenreViewModel> Genre { get; set; } = null!;
    
    public PublisherViewModel? Publisher { get; set; }
    
    public IEnumerable<string> Type { get; set; } = null!;

    [DisplayName("Created At")]
    public DateTime CreatedAt { get; set; }

    public string? ImageName { get; set; }

    public string? SignedUrl { get; set; }

    [DataType(DataType.Date)]
    [DisplayName("Publish Date")]
    public DateTime PublishDate { get; set; }

    [DisplayName("ISBN")]
    public string? Isbn { get; set; }
    
    [MaxLength(5000)]
    public string? Description { get; set; }
}