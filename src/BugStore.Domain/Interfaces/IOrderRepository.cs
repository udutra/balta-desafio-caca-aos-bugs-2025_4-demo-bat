using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces;

public interface IOrderRepository{
    Task<Order?> CreateOrderAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
}