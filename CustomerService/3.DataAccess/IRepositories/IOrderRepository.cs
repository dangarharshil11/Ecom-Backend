using CustomerService._3.DataAccess.Domains;
using CustomerService._4.Infrastructure.Models;

namespace CustomerService._3.DataAccess.IRepositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(Order order);
        Task<Order> UpdateOrder(Order existingOrder, Order updatedOrder);
        Task<Order?> GetOrderById(Guid Id);
        Task<List<Order>> GetOrdersByUserId(Guid userId);
        Task<List<Order>> GetOrders(FilterModel filter);
    }
}
