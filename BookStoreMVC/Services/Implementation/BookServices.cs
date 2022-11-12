using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;

namespace BookStoreMVC.Services.Implementation;

public class BookServices : IBookRepository
{
    public IEnumerable<Book> GetAll()
    {
        throw new NotImplementedException();
    }

    public Book GetById()
    {
        throw new NotImplementedException();
    }

    public Book Add(BookViewModel bookViewModel)
    {
        throw new NotImplementedException();
    }

    public Task<Book> AddAsync(BookViewModel bookViewModel)
    {
        throw new NotImplementedException();
    }

    public Book Update(BookViewModel bookViewModel)
    {
        throw new NotImplementedException();
    }

    public Task<Book> UpdateAsync(BookViewModel bookViewModel)
    {
        throw new NotImplementedException();
    }

    public Book Delete(string bookId)
    {
        throw new NotImplementedException();
    }

    public Task<Book> DeleteAsync(string bookId)
    {
        throw new NotImplementedException();
    }
}