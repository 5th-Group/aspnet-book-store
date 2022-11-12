using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IBookTypeRepository
{
    IEnumerable<BookType> GetAll();

    BookType GetById(string bookTypeId);

    BookType Add();

    BookType Update(string bookTypeId);

    BookType Delete(string bookTypeId);
}