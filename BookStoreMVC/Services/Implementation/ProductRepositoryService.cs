using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class ProductRepositoryService : IProductRepository
{
    private IMongoCollection<Product> _collection;
    private ProjectionDefinition<Product> _projectionDefinition = Builders<Product>.Projection.Combine();

    public ProductRepositoryService(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(dataAccess.Value.ConnectionString);

        var database = mongoClient.GetDatabase(dataAccess.Value.DatabaseName);

        _collection = database.GetCollection<Product>(dataAccess.Value.ProductCollectionName);
    }

    public IEnumerable<Product> GetAll(ProjectionDefinition<Product>? projection = null, string filter = "_")
    {
        _projectionDefinition = projection ?? Builders<Product>.Projection.Combine();
        return _collection.Find(filter => true).Project(_projectionDefinition).ToList().Select(v => BsonSerializer.Deserialize<Product>(v));
    }

    public Product GetById(string productId)
    {
        // return _collection.Find($"_id: {productId}").FirstOrDefault();
        return _collection.Find(x => x.Id == productId).FirstOrDefault();
    }

    public Product GetByFilter(FilterDefinition<Product> filterDefinition)
    {
        return _collection.Find(filterDefinition).FirstOrDefault();
    }

    public async Task AddAsync(Product model)
    {
        await _collection.InsertOneAsync(model);
    }
}