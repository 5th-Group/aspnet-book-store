using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Mapper;

public class MapAuthor
{
    public static AuthorViewModel MapAuthorViewModel(Author author)
    {
        return new AuthorViewModel
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            Initials = author.Initials,
            Description = author.Description
        };
    }
}