using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class BookServices : IBookRepository
{
    private readonly IMongoCollection<Book> _bookCollection;
    public BookServices(IOptions<BookStoreDataAccess> dataAccess)
    {
        
        var mongoClient = new MongoClient(
            dataAccess.Value.ConnectionString);
        
        var mongoDatabase = mongoClient.GetDatabase(
            dataAccess.Value.DatabaseName);
        
        _bookCollection = mongoDatabase.GetCollection<Book>(
            dataAccess.Value.BookCollectionName);
    }
    public IEnumerable<Book> GetAll(string filter)
    {
        return _bookCollection.Find( filter => true).ToEnumerable();
    }

    public Book GetById(string bookId)
    {
        return _bookCollection.Find( $"_id: {ObjectId.Parse(bookId)}").FirstOrDefault();
    }

    public IActionResult Add(Book book)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public IActionResult Update(Book book)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public IActionResult Delete(string bookId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string bookId)
    {
        throw new NotImplementedException();
    }
}