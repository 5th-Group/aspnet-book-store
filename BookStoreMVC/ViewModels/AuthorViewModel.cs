namespace BookStoreMVC.ViewModels;

public class AuthorViewModel
{
    public string? Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Initials { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}