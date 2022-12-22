using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class ReviewRepositoryService : IReviewRepository
{
    private readonly IMongoCollection<Review> _collection;
    // private readonly ProjectionDefinition<Review> _projectionDefinition = Builders<Review>.

    public ReviewRepositoryService(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(dataAccess.Value.ConnectionString);

        var database = mongoClient.GetDatabase(dataAccess.Value.DatabaseName);
        
        _collection = database.GetCollection<Review>(dataAccess.Value.ReviewCollectionName);
    }

    public IEnumerable<Review> GetAll()
    {

        return _collection.Find(_ => true).ToEnumerable();
    }

    public Review GetById(string reviewId)
    {
        return _collection.Find(r => r.Id == reviewId).FirstOrDefault();
    }
}