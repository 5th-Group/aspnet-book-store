using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.ViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
    
}