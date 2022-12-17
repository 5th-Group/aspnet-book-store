using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class ProductRepositoryService : IProductRepository
{
    private IMongoCollection<Product> _collection;

    public ProductRepositoryService(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(dataAccess.Value.ConnectionString);

        var database = mongoClient.GetDatabase(dataAccess.Value.DatabaseName);

        _collection = database.GetCollection<Product>(dataAccess.Value.ProductCollectionName);
    }

    public IEnumerable<Product> GetAll()
    {
        return _collection.Find(_ => true).ToEnumerable();
    }

    public Product GetById(string productId)
    {
        // return _collection.Find($"_id: {productId}").FirstOrDefault();
        return _collection.Find(x => x.Id == productId).FirstOrDefault();
    }

    public async Task AddAsync(Product model)
    {
        await _collection.InsertOneAsync(model);
    }
}