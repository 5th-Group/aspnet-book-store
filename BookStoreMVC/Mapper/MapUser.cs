using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Mapper;

public class MapUser
{
    public static UserViewModel MapUserViewModel(User user)
    {
        return new UserViewModel
        {
            Id = user.Id.ToString(),
            Username = user.UserName,
        };
    }
}