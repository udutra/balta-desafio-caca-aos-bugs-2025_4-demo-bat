using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Product.Requests;
using BugStore.Domain.Entities;

namespace BugStore.Application.Interfaces;

public interface IProductService{
    Task<Response<Product>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<Response<Product>> GetProductByIdAsync(GetProductByIdRequest request, CancellationToken cancellationToken);
    Task<PagedResponse<List<Product>?>> GetAllProductsAsync(GetAllProductsRequest request, CancellationToken cancellationToken);
    Task<Response<Product>> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken);
    Task<Response<Product>> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken);
}