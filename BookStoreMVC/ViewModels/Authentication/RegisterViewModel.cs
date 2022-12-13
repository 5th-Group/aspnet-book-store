using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.ViewModels.Authentication;

public class RegisterViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Username can not be empty")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password can not be empty")]
    [PasswordPropertyText]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,20}$", ErrorMessage = "Mật khẩu ít nhất 8 kí tự, phải có 1 kí tự đặc biệt, chữ số và kí tự viết hoa")]
    public string Password { get; set; } = null!;
    
    [DisplayName("Confirm Password")]
    [Required(ErrorMessage = "Password confirmation can not be empty")]
    [PasswordPropertyText]
    [Compare("Password", ErrorMessage = "Password does not match")]
    public string ConfirmPassword { get; set; } = null!;
    
    [DisplayName("First Name")]
    [Required(ErrorMessage = "First Name can not be empty")]
    public string FirstName { get; set; } = null!;
    
    [DisplayName("Last Name")]
    [Required(ErrorMessage = "Last Name can not be empty")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Please choose your gender")]
    public string Gender { get; set; } = string.Empty;

    public IList<SelectListItem> GenderList = new List<SelectListItem>
    {
        new("Male", "male"),
        new("Female", "female"),
        new("Other", "other")
    };

    // public IEnumerable<UserAddress> Address { get; set; }
    public string AddressType { get; set; }

    public string Address { get; set; }

    // public IList<UserAddress> Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    // public string Role { get; set; } = null!;

    public string? Country { get; set; }

    public string PhoneNumber { get; set; } = null!;
}