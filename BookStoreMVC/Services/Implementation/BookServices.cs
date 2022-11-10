using BookStoreMVC.Models;

namespace BookStoreMVC.Services.Implementation;

public class BookService : IBookRepository
{
    public IEnumerable<Book> GetAll()
    {
        throw new NotImplementedException();
    }

    public Book GetById()
    {
        throw new NotImplementedException();
    }

    public Book Add(string bookId)
    {
        throw new NotImplementedException();
    }

    Task<Book> IBookRepository.AddAsync(string bookId)
    {
        throw new NotImplementedException();
    }

    public Book AddAsync(string bookId)
    {
        throw new NotImplementedException();
    }

    public Book Update(string bookId)
    {
        throw new NotImplementedException();
    }

    public Task<Book> UpdateAsync(string bookId)
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