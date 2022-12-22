using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class LanguageServices : ILanguageRepository
{
    private readonly IMongoCollection<Language> _languageCollection;
    private ProjectionDefinition<Language> _projectionDefinition = Builders<Language>.Projection.Include(doc => true);
    private FilterDefinition<Language> _filterDefinition = Builders<Language>.Filter.Empty; 
    
    public LanguageServices(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(dataAccess.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(dataAccess.Value.DatabaseName);

        _languageCollection = mongoDatabase.GetCollection<Language>(dataAccess.Value.LanguageCollectionName);

    }
    public IEnumerable<Language> GetAll(FilterDefinition<Language>? filterDefinition = null, ProjectionDefinition<Language>? projectionDefinition = null)
    {
        _filterDefinition = filterDefinition ?? Builders<Language>.Filter.Empty;
        _projectionDefinition = projectionDefinition ?? Builders<Language>.Projection.Combine();

        return _languageCollection.Find(_filterDefinition).Project(_projectionDefinition).ToList().Select(l => BsonSerializer.Deserialize<Language>(l));
    }

    public async Task<Language> GetByIdAsync(string languageId, ProjectionDefinition<Language>? projectionDefinition = null)
    {
        _projectionDefinition = projectionDefinition ?? Builders<Language>.Projection.Combine();

        var lang = await _languageCollection.Find(l => l.Id == languageId).Project(_projectionDefinition)
            .FirstOrDefaultAsync();
        
        return BsonSerializer.Deserialize<Language>(lang);
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