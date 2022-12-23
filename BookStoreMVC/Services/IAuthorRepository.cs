using BookStoreMVC.Models;
using MongoDB.Driver;

namespace BookStoreMVC.Services;

public interface IAuthorRepository
{
    IEnumerable<Author> GetAll();

    Task<Author> GetById(string authorId);

    Task<Author> GetWithFilterAsync(FilterDefinition<Author> filterDefinition,
        ProjectionDefinition<Author>? projectionDefinition = null);

    // Author Add();

    Task AddAsync(Author author);

    Task DeleteAsync(string authorId);

    Task UpdateAsync(string authorId);
}