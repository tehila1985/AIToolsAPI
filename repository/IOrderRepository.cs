
using Repository.Models;

namespace Repository
{
  public interface IOrderRepository
  {
    Task<List<Order>> GetOrdersUser(int id);
    Task<Order> AddNewOrder(Order order);
    Task<Order?> GetOrderById(int id);
  }
}