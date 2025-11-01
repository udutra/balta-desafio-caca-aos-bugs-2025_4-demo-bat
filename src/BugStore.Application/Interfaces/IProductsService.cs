using BugStore.Domain.Entities;

namespace BugStore.Application.Interfaces;

public interface IProductsService{
    Task<(Product? product, string message)> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(List<Product> products, string message)> GetAllAsync(CancellationToken cancellationToken);
    Task<(Product? product, bool success, string message)> CreateAsync(Product product, CancellationToken cancellationToken);
    Task<(Product? product, bool success, string message)> UpdateAsync(Product product, CancellationToken cancellationToken);
    Task<(bool success, string message)> DeleteAsync(Guid id, CancellationToken cancellationToken);
}