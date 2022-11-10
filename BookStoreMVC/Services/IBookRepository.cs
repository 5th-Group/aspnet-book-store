using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IBookRepository
{
    IEnumerable<Book> GetAll();

    Book GetById();

    Book Add(string bookId);
    
    Task<Book> AddAsync(string bookId);

    Book Update(string bookId);

    Task<Book> UpdateAsync(string bookId);

    Book Delete(string bookId);

    Task<Book> DeleteAsync(string bookId);
}