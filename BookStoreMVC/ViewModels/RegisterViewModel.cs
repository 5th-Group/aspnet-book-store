using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BookStoreMVC.Models;

namespace BookStoreMVC.ViewModels;

public class RegisterViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Username can not be empty")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password can not be empty")]
    [PasswordPropertyText]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,20}$", ErrorMessage = "Mật khẩu ít nhất 8 kí tự, phải có 1 kí tự đặc biệt, chữ số và kí tự viết hoa")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Password confirmation can not be empty")]
    [PasswordPropertyText]
    [Compare("Password", ErrorMessage = "Password does not match")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "First Name can not be empty")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last Name can not be empty")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Please choose your gender")]
    public string[] Gender = { "Male, Female, Other" };


    // public IList<UserAddress> Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Country { get; set; }

    public string PhoneNumber { get; set; } = null!;
}