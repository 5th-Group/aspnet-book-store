using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace BookStoreMVC.ViewModels.Book;

public class IndexBookViewModel
{
    public string? Id { get; set; }

    public string Title { get; set; } = null!;

    public AuthorViewModel Author { get; set; } = null!;
    public string authorDisplay
    {
        get
        {
            return this.Author.FirstName + " " + this.Author.LastName;
        }
    }

    public LanguageViewModel? Language { get; set; }

    public IEnumerable<BookGenreViewModel> Genre { get; set; } = null!;

    public PublisherViewModel? Publisher { get; set; }

    public IEnumerable<string> Type { get; set; } = null!;

    [DisplayName("Created At")]
    public DateTime CreatedAt { get; set; }

    public string? ImageName { get; set; }

    public string? SignedUrl { get; set; }

    [MaxLength(5000)]
    public string? Description { get; set; }
}