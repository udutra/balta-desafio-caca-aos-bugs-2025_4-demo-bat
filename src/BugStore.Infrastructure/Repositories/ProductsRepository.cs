using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;

namespace BugStore.Infrastructure.Repositories;

public class ProductsRepository : IProductRepository {
    public Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task AddProductAsync(Product product, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task UpdateProductAsync(Product product, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task DeleteProductAsync(Product product, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
}