using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IAuthorRepository
{
    IEnumerable<Author> GetAll();

    Task<Author> GetById(string authorId);

    // Author Add();

    Task AddAsync(Author author);

    Task DeleteAsync(string authorId);

    Task UpdateAsync(string authorId);
}