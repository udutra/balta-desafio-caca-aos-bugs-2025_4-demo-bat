using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository {
    public async Task<Order?> CreateOrderAsync(Order order, CancellationToken cancellationToken){
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }
    public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken){
        if (id == Guid.Empty)
            return null;

        return await context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Lines)
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}