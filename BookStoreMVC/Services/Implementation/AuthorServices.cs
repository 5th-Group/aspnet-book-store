using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class AuthorServices : IAuthorRepository
{
    private readonly IMongoCollection<Author> _authorCollection;
    public AuthorServices(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(
            dataAccess.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dataAccess.Value.DatabaseName);

        _authorCollection = mongoDatabase.GetCollection<Author>(
            dataAccess.Value.AuthorCollectionName);
    }
    public IEnumerable<Author> GetAll()
    {
        return _authorCollection.Find(_ => true).ToEnumerable();
    }

    public Author GetById(string authorId)
    {
        throw new NotImplementedException();
    }
    //
    // public Author Add()
    // {
    //     throw new NotImplementedException();
    // }

    public async Task AddAsync(Author author)
    {
        try
        {
            await _authorCollection.InsertOneAsync(author);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

    }

    public async Task DeleteAsync(string id) =>
       await _authorCollection.DeleteOneAsync(x => x.Id == id);

    public Task UpdateAsync(string authorId)
    {
        throw new NotImplementedException();
    }
}