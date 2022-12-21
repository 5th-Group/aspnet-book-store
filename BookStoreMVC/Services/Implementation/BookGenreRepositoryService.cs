using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class BookGenreRepositoryService : IBookGenreRepository
{
    private readonly IMongoCollection<BookGenre> _bookGenreCollection;
    public BookGenreRepositoryService(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(dataAccess.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(dataAccess.Value.DatabaseName);

        _bookGenreCollection = mongoDatabase.GetCollection<BookGenre>(dataAccess.Value.BookGenreCollectionName);

    }
    public IEnumerable<BookGenre> GetAll()
    {
        return _bookGenreCollection.Find(_ => true).ToEnumerable();

    }

    public BookGenre GetById(string bookGenreId)
    {
        var resutl = _bookGenreCollection.Find(x => x.Id == bookGenreId).FirstOrDefaultAsync().Result;
        return resutl;
    }

    public async Task AddAsync(BookGenre model)
    {
        try
        {
            await _bookGenreCollection.InsertOneAsync(model);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

    }

    public async Task UpdateAsync(BookGenre model)
    {
        try
        {
            await _bookGenreCollection.ReplaceOneAsync($"_id: {ObjectId.Parse(model.Id)}", model);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteAsync(string bookGenreId)
    {
        await _bookGenreCollection.DeleteOneAsync(x => x.Id == bookGenreId);
    }
}