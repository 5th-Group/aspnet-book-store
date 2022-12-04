using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IPublisherRepository
{
    IEnumerable<Publisher> GetAll();

    Publisher GetById(string publisherId);

    Publisher Add();

    Task AddAsync(Publisher document);

    Publisher Update(string publisherId);

    Task DeleteAsync(string publisherId);
}