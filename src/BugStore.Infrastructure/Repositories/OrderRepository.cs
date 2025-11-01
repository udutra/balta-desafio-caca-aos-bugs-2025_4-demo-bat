using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;

namespace BugStore.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository {
    public Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task AddOrderAsync(Order order, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
}