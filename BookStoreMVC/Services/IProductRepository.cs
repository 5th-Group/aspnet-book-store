using BookStoreMVC.Models;
using MongoDB.Driver;

namespace BookStoreMVC.Services;

public interface IProductRepository
{
    IEnumerable<Product> GetAll(ProjectionDefinition<Product>? projection = null, string filter = "_");

    Product GetById(string productId);
    
    Task<Product> GetByFilterAsync(FilterDefinition<Product> filterDefinition, ProjectionDefinition<Product>? projection = null);
    
    Product GetByFilter(FilterDefinition<Product> filterDefinition);

    Task AddAsync(Product model);
}