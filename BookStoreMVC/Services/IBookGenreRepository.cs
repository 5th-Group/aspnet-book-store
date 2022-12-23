using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IBookGenreRepository
{
    IEnumerable<BookGenre> GetAll();
    Task<BookGenre> GetById(string bookGenreId);

    Task AddAsync(BookGenre model);

    Task UpdateAsync(BookGenre model);

    Task DeleteAsync(string bookGenreId);
}