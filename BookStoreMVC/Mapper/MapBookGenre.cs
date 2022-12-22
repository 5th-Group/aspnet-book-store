using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Mapper;

public class MapBookGenre
{
    public static IEnumerable<BookGenreViewModel> MapManyBookGenreViewModels(IEnumerable<BookGenre> bookGenres)
    {
        return bookGenres.Select(genre => new BookGenreViewModel
        {
            Id = genre.Id,
            Name = genre.Name
        });
    }
}