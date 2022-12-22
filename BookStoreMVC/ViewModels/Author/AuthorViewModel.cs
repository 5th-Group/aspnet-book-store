using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.ViewModels;

public class AuthorViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "This field is required !")]
    [DisplayName("First Name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "This field is required !")]
    [DisplayName("Last Name")]
    public string LastName { get; set; } = null!;

    [DisplayName("Initials")]
    public string Initials { get; set; } = string.Empty;
    
    public string DisplayName => $"{FirstName} {Initials} {LastName}";

    [DisplayName("Description")]
    public string Description { get; set; } = string.Empty;
}