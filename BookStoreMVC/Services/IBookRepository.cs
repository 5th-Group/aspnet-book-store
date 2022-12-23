using BookStoreMVC.Models;
using MongoDB.Driver;

namespace BookStoreMVC.Services;

public interface IBookRepository
{
    IEnumerable<Book> GetAll(FilterDefinition<Book>? filterDefinition = null, ProjectionDefinition<Book>? projectionDefinition = null);

    Task<Book> GetById(string bookId, ProjectionDefinition<Book>? projectionDefinition = null);
    
    Task<Book> GetByFilterAsync(FilterDefinition<Book> filterDefinition, ProjectionDefinition<Book>? projectionDefinition = null);
    

    Task AddAsync(Book book);
    

    Task UpdateAsync(Book book);
    

    Task DeleteAsync(string bookId);

}