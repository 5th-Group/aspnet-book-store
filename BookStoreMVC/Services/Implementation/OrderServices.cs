using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreMVC.Services.Implementation;

public class OrderService : IOrderRepository
{
    private readonly IMongoCollection<Order> _orderCollection;
    public OrderService(IOptions<BookStoreDataAccess> dataAccess)
    {

        var mongoClient = new MongoClient(
            dataAccess.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dataAccess.Value.DatabaseName);

        _orderCollection = mongoDatabase.GetCollection<Order>(
            dataAccess.Value.OrderCollectionName);
    }
    public IEnumerable<Order> GetAll()
    {
        return _orderCollection.Find(_ => true).ToEnumerable();
    }

    public async Task<Order> GetByOrderId(string orderId)
    {
        var resutl = await _orderCollection.Find(x => x.Id == orderId).FirstOrDefaultAsync();
        return resutl;
    }
    public IEnumerable<Order> GetByUserId(string userId)
    {
        var resutl = _orderCollection.Find(x => x.Customer == userId).ToEnumerable();
        return resutl;
    }

    public async Task AddAsync(Order order)
    {
        await _orderCollection.InsertOneAsync(order);
    }

    public Order Add()
    {
        throw new NotImplementedException();
    }

    public Order Update(string orderId)
    {
        throw new NotImplementedException();
    }

    public Order Delete(string orderId)
    {
        throw new NotImplementedException();
    }
}