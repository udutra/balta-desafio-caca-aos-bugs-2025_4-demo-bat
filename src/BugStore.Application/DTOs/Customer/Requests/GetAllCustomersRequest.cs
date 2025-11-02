namespace BugStore.Application.DTOs.Customer.Requests;

public class GetAllCustomersRequest(
    int pageNumber = Configuration.DefaultPageNumber,
    int pageSize = Configuration.DefaultPageSize)
    : PagedRequest(pageNumber, pageSize);