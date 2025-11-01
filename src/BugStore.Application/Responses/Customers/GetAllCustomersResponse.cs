using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Customers;

public class GetAllCustomersResponse : PagedResponse<List<Customer>>
{
    public GetAllCustomersResponse(List<Customer>? data, int totalCount, int currentPage, int pageSize)
        : base(data, totalCount, currentPage, pageSize)
    {
    }

    public GetAllCustomersResponse(List<Customer>? data, int statusCode = Configuration.DefaultStatusCode,
        string message = "Lista de clientes retornada com sucesso.")
        : base(data, statusCode, message)
    {
    }
}