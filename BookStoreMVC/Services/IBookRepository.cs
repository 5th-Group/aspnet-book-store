using BookStoreMVC.Models;
using MongoDB.Driver;

namespace BookStoreMVC.Services;

public interface IBookRepository
{
    IEnumerable<Book> GetAll(FilterDefinition<Book>? filterDefinition = null, ProjectionDefinition<Book>? projectionDefinition = null);

    Task<Book> GetById(string bookId, ProjectionDefinition<Book>? projectionDefinition = null);

    // IActionResult Add(Book book);

    Task AddAsync(Book book);

    // IActionResult Update(Book book);

    Task UpdateAsync(Book book);

    // IActionResult Delete(string bookId);

    Task DeleteAsync(string bookId);

}