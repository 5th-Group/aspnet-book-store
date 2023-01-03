using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.ViewModels.Admin;

public class AddUserViewModel
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    
    public string Gender { get; set; } = null!;
    
    public IList<SelectListItem> GenderList = new List<SelectListItem>
    {
        new("Male", "male"),
        new("Female", "female"),
        new("Other", "other")
    };

    public string? Country { get; set; }

    public string AddressType { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Role { get; set; } = null!;
}