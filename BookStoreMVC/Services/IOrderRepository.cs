using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();

    Task<Order> GetByOrderId(string orderId);
    IEnumerable<Order> GetByUserId(string userId);

    public Task AddAsync(Order order);


    Order Add();

    Order Update(string orderId);

    Order Delete(string orderId);
}