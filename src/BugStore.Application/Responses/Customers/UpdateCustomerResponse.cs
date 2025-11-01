using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Customers;

public class UpdateCustomerResponse(Customer? data, int statusCode = Configuration.DefaultStatusCode,
    string message = "Cliente atualizado com sucesso.")
    : Response<Customer>(data, statusCode, message);