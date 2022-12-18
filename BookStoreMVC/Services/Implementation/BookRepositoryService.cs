using BookStoreMVC.DataAccess;
using BookStoreMVC.Exceptions;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class BookRepositoryService : IBookRepository
{
    private readonly IMongoCollection<Book> _bookCollection;
    private readonly IMongoCollection<Product> _productCollection;

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

    public IEnumerable<Book> GetAll(string filter)
    {
        return _bookCollection.Find(filter => true).ToEnumerable();
    }

    public async Task<Book> GetById(string bookId)
    {
        return await _bookCollection.Find(x => x.Id == bookId).FirstOrDefaultAsync();
    }

    public IActionResult Add(Book book)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Book book)
    {
        await _bookCollection.InsertOneAsync(book);
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

    public async Task DeleteAsync(string id)
    {
        if (_productCollection.Find(p => p.BookId == id).FirstOrDefault() is null)
        {
            await _bookCollection.DeleteOneAsync(b => b.Id == id);

        }
        else throw new BookIsReferencedByOtherException("The book is being referenced by a product entity", id);
    }
}