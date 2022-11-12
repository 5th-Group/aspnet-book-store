using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IAuthorRepository
{
    IEnumerable<Author> GetAll();

    Author GetById(string authorId);

    Author Add();

    Task<Author> AddAsync();

    Author Delete(string authorId);

    Author Update(string authorId);
}