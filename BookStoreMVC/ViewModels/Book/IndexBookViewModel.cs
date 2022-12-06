using BookStoreMVC.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.ViewModels;


public class IndexBookViewModel
{
    public string? Id { get; set; }

    public string Title { get; set; } = null!;

    public int PageCount { get; set; }

    public Author? AuthorDisplay { get; set; }

    public string? Language { get; set; }

    public IList<string> Genre { get; set; } = null!;
    public string? Publisher { get; set; }
    public IList<string> Type { get; set; } = null!;


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