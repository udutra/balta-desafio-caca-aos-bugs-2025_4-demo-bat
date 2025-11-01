using BugStore.Application.Interfaces;
using BugStore.Domain.Entities;

namespace BugStore.Application.Services;

public class ProductsService : IProductsService {
    public Task<(Product? product, string message)> GetByIdAsync(Guid id, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<(List<Product> products, string message)> GetAllAsync(CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<(Product? product, bool success, string message)> CreateAsync(Product product, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<(Product? product, bool success, string message)> UpdateAsync(Product product, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<(bool success, string message)> DeleteAsync(Guid id, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
}