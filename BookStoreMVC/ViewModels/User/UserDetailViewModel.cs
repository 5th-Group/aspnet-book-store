using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.ViewModels;

public class UserDetailViewModel
{
    [DisplayName("User name")]
    public string? Username { get; set; }
    [DisplayName("First name")]
    public string? Firstname { get; set; }
    [DisplayName("Last name")]
    public string? Lastname { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }

    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; } = null!;


}