using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces;

public interface IOrderRepository{
    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddOrderAsync(Order order, CancellationToken cancellationToken);
}