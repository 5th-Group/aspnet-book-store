namespace BookStoreMVC.Models;

public class Contact
{
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Twitter { get; set; }
    public string? Facebook { get; set; }
    public string Email { get; set; } = null!;
}