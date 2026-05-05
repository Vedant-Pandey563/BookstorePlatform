using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Abstractions;

// Application depends on this interface only.
// Infrastructure will provide the Dapper implementation.
public interface IOrderRepository
{
    Task<Order?> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<List<OrderItem>> GetItemsByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(int orderId, CancellationToken cancellationToken = default);
}
