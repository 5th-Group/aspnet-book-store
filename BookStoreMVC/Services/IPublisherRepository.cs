using BookStoreMVC.Models;
using MongoDB.Driver;

namespace BookStoreMVC.Services;

public interface IPublisherRepository
{
    IEnumerable<Publisher> GetAll();

    Publisher GetById(string publisherId);

    Task<Publisher> GetWithFilterAsync(FilterDefinition<Publisher> filterDefinition);

    Publisher Add();

    Task AddAsync(Publisher document);

    Publisher Update(string publisherId);

    Task DeleteAsync(string publisherId);
}