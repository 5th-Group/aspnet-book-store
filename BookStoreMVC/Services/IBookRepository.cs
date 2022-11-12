using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Services;

public interface IBookRepository
{
    IEnumerable<Book> GetAll();

    Book GetById();

    Book Add(BookViewModel bookViewModel);
    
    Task<Book> AddAsync(BookViewModel bookViewModel);

    Book Update(BookViewModel bookViewModel);

    Task<Book> UpdateAsync(BookViewModel bookViewModel);

    Book Delete(string bookId);

    Task<Book> DeleteAsync(string bookId);
    
}