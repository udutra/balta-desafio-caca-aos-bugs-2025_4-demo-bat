namespace BugStore.Application.Requests.Products;

public class GetAllProductsRequest(
    int pageNumber = Configuration.DefaultPageNumber,
    int pageSize = Configuration.DefaultPageSize)
    : PagedRequest(pageNumber, pageSize);