using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Customers;

public class GetAllCustomersResponse(List<Customer>? data, int totalCount, int currentPage, int pageSize,
    int statusCode = Configuration.DefaultStatusCode, string? message = "Lista de clientes retornada com sucesso.")
    : PagedResponse<List<Customer>>(data, totalCount, statusCode, currentPage, pageSize, message){
}