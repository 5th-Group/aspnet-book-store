using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class LanguageServices : ILanguageRepository
{
    private readonly IMongoCollection<Language> _languageCollection;
    public LanguageServices(IConfiguration config, IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(dataAccess.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(dataAccess.Value.DatabaseName);

        _languageCollection = mongoDatabase.GetCollection<Language>(dataAccess.Value.LanguageCollectionName);

    }
    public IEnumerable<Language> GetAll()
    {
        throw new NotImplementedException();
    }

    public Language GetById(string languageId)
    {
        throw new NotImplementedException();
    }


    public async Task AddAsync(Language model)
    {
        try
        {
            await _languageCollection.InsertOneAsync(model);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public Task UpdateAsync(Language model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string languageId)
    {
        throw new NotImplementedException();
    }
}