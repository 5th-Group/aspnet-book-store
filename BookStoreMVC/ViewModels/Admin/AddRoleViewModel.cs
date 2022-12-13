using System.ComponentModel;
using Microsoft.Build.Framework;

namespace BookStoreMVC.ViewModels.Admin;

public class AddRoleViewModel
{
    [Required]
    [DisplayName("Name Role of Your Mom")]
    public string Name { get; set; } = null!;
}