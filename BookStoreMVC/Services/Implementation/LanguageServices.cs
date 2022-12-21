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
        return _languageCollection.Find(_ => true).ToEnumerable();
    }

    public Language GetById(string languageId)
    {
        return _languageCollection.Find(x => x.Id == languageId).FirstOrDefaultAsync().Result;
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

    public async Task DeleteAsync(string languageId)
    {
        await _languageCollection.DeleteOneAsync(x => x.Id == languageId);

    }
}