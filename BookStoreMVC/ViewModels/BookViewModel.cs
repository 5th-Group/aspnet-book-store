namespace BookStoreMVC.ViewModels;

public class BookViewModel
{
    public string? Id { get; set; }

    public string Title { get; set; } = null!;

    public int PageCount { get; set; }
    
    public string? Author { get; set; }
    
    public string? Language { get; set; }
    
    public IList<string>? Genre { get; set; }
    
    public IList<string>? Type { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public string ImageUri { get; set; } = null!;
    
    public DateTime PublishDate { get; set; }
    
    public string? Publisher { get; set; }

    public string? Isbn { get; set; }

    public string? Description { get; set; }
}