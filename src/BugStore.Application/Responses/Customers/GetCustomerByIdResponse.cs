using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Customers;

public class GetCustomerByIdResponse(Customer? data, int statusCode = Configuration.DefaultStatusCode,
    string message = "Cliente localizado com sucesso.")
    : Response<Customer>(data, statusCode, message);