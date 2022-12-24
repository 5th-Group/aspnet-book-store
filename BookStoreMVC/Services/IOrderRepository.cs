using BookStoreMVC.Models.Payment;
using MongoDB.Driver;

namespace BookStoreMVC.Services;

public interface IOrderRepository
{
    // IEnumerable<Order> GetAll();
    //
    IEnumerable<Order> GetAll();

    Task<Order?> GetByOrderId(string orderId);
    IEnumerable<Order> GetByUserId(string userId);
    
    Task<Order> GetByFilterAsync(FilterDefinition<Order> filterDefinition, ProjectionDefinition<Order>? projectionDefinition = null);
    
    public Task AddAsync(Order order);
    
    Order Add();
    
    Task UpdateAsync(Order order);
    
    Order Delete(string orderId);
}