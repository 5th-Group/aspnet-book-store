using BookStoreMVC.Models;

namespace BookStoreMVC.Services;

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();

    Order GetById(string orderId);

    Order Add();

    Order Update(string orderId);

    Order Delete(string orderId);
}