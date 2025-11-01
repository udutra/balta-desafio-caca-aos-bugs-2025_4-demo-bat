using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Products;

public class GetAllProductsResponse : PagedResponse<List<Product>>
{
    public GetAllProductsResponse(List<Product>? data, int totalCount, int currentPage, int pageSize)
        : base(data, totalCount, currentPage, pageSize)
    {
    }

    public GetAllProductsResponse(List<Product>? data, int statusCode = Configuration.DefaultStatusCode, string? message = null)
        : base(data, statusCode, message)
    {
    }
}