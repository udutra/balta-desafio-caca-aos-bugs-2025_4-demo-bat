namespace BugStore.Application.DTOs.Product.Requests;

public class GetAllProductsRequest(
    int pageNumber = Configuration.DefaultPageNumber,
    int pageSize = Configuration.DefaultPageSize)
    : PagedRequest(pageNumber, pageSize);