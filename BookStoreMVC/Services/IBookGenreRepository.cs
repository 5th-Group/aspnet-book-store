using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IBookGenreRepository
{
    IEnumerable<BookGenre> GetAll();
    BookGenre GetById(string bookGenreId);

    BookGenre Add();

    BookGenre Update(string bookGenreId);

    BookGenre Delete(string bookGenreId);
}