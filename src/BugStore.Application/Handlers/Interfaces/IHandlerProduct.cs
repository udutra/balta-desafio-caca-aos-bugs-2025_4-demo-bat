using BugStore.Application.Requests.Products;
using BugStore.Application.Responses.Products;

namespace BugStore.Application.Handlers.Interfaces;

public interface IHandlerProduct{
    Task<CreateProductResponse> CreateProductAsync(CreateProductRequest request,
        CancellationToken cancellationToken = default);
    Task<GetProductByIdResponse> GetProductByIdAsync(GetProductByIdRequest request,
        CancellationToken cancellationToken = default);
    Task<GetAllProductsResponse> GetAllProductsAsync(GetAllProductsRequest request,
        CancellationToken cancellationToken = default);
    Task<DeleteProductResponse> DeleteProductAsync(DeleteProductRequest request,
        CancellationToken cancellationToken = default);
    Task<UpdateProductResponse> UpdateProductAsync(UpdateProductRequest request,
        CancellationToken cancellationToken = default);

}