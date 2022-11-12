using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IPublisherRepository
{
    IEnumerable<Publisher> GetAll();

    Publisher GetById(string publisherId);

    Publisher Add();

    Publisher Update(string publisherId);

    Publisher Delete(string publisherId);
}