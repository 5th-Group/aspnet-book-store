using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class PublisherService : IPublisherRepository
{
    private readonly IMongoCollection<Publisher> _publisherCollection;
    public PublisherService(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(dataAccess.Value.ConnectionString);
        var database = mongoClient.GetDatabase(dataAccess.Value.DatabaseName);
        _publisherCollection = database.GetCollection<Publisher>(dataAccess.Value.PublisherCollectionName);
    }
    public IEnumerable<Publisher> GetAll()
    {
        return _publisherCollection.Find(_ => true).ToEnumerable();
    }

    public Publisher GetById(string publisherId)
    {
        throw new NotImplementedException();
    }

    public Publisher Add()
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Publisher document)
    {
        try
        {
            await _publisherCollection.InsertOneAsync(document);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public Publisher Update(string publisherId)
    {
        throw new NotImplementedException();
    }

    public Publisher Delete(string publisherId)
    {
        throw new NotImplementedException();
    }
}