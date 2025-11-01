using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Customers;

public class CreateCustomerResponse(Customer? data, int statusCode = Configuration.DefaultStatusCode,
    string message = "Cliente criado com sucesso.") : Response<Customer>(data, statusCode, message);