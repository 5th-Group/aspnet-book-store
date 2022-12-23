using BookStoreMVC.DataAccess;
using BookStoreMVC.Exceptions;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class BookRepositoryService : IBookRepository
{
    private readonly IMongoCollection<Book> _bookCollection;
    private readonly IMongoCollection<Product> _productCollection;
    private ProjectionDefinition<Book> _projectionDefinition = Builders<Book>.Projection.Combine();
    private FilterDefinition<Book> _filterDefinition = Builders<Book>.Filter.Empty;


    public BookRepositoryService(IOptions<BookStoreDataAccess> dataAccess)
    {
        var mongoClient = new MongoClient(
            dataAccess.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dataAccess.Value.DatabaseName);

        _bookCollection = mongoDatabase.GetCollection<Book>(
            dataAccess.Value.BookCollectionName);
        
        _productCollection = mongoDatabase.GetCollection<Product>(
            dataAccess.Value.ProductCollectionName);
    }

    public IEnumerable<Book> GetAll(FilterDefinition<Book>? filterDefinition = null, ProjectionDefinition<Book>? projectionDefinition = null)
    {
        _filterDefinition = filterDefinition ?? Builders<Book>.Filter.Empty;
        _projectionDefinition = projectionDefinition ?? Builders<Book>.Projection.Combine();

        return _bookCollection.Find(_filterDefinition).Project(_projectionDefinition).ToList().Select(b => BsonSerializer.Deserialize<Book>(b));
    }

    public async Task<Book> GetById(string bookId, ProjectionDefinition<Book>? projectionDefinition = null)
    {
        _projectionDefinition = projectionDefinition ?? Builders<Book>.Projection.Combine();
        
        var item = await _bookCollection.Find(b => b.Id == bookId).Project(_projectionDefinition).FirstOrDefaultAsync();
        return BsonSerializer.Deserialize<Book>(item);
    }
    
    public Task<Book> GetByFilterAsync(FilterDefinition<Book> filterDefinition,
        ProjectionDefinition<Book>? projectionDefinition = null)
    {
        return _bookCollection.Find(filterDefinition).FirstOrDefaultAsync();
    }
    

    public async Task AddAsync(Book book)
    {
        await _bookCollection.InsertOneAsync(book);
    }
    

    public Task UpdateAsync(Book book)
    {
        throw new NotImplementedException();
    }
    

    public async Task DeleteAsync(string id)
    {
        if (_productCollection.Find(p => p.BookId == id).FirstOrDefault() is null)
        {
            await _bookCollection.DeleteOneAsync(b => b.Id == id);

        }
        else throw new BookIsReferencedByOtherException("The book is being referenced by a product entity", id);
    }
}