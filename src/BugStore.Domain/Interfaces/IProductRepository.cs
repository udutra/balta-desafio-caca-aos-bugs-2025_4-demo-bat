using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces;

public interface IProductRepository{
    Task<Product?> CreateProductAsync(Product product, CancellationToken cancellationToken);
    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product?> GetProductBySlugAsync(string slug, CancellationToken cancellationToken);
    IQueryable<Product> GetAllProducts();
    Task<int> UpdateProductAsync(Product product, CancellationToken cancellationToken);
    Task<int> DeleteProductAsync(Product product, CancellationToken cancellationToken);
}