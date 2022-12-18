using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.ViewModels;

public class ChangePasswordViewModel
{

    [Required(ErrorMessage = "Old password is required!")]
    [DisplayName("Old password")]
    public string? OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,20}$", ErrorMessage = "Password have at least 8 characters and 1 special character")]
    [DisplayName("New password")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm is required!")]
    [Compare("NewPassword", ErrorMessage = "Password doesn't match!")]
    [DisplayName("Confirm password")]
    public string? PasswordConfirm { get; set; }
}