using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces;

public interface IProductRepository{
    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken);
    Task AddProductAsync(Product product, CancellationToken cancellationToken);
    Task UpdateProductAsync(Product product, CancellationToken cancellationToken);
    Task DeleteProductAsync(Product product, CancellationToken cancellationToken);
}