using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();

    Product GetById(string productId);

    Task AddAsync(Product model);
}