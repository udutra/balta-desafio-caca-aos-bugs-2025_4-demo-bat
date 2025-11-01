using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Products;

public class GetProductByIdResponse(Product? data, int statusCode = Configuration.DefaultStatusCode,
    string message = "Produto encontrado com sucesso.") : Response<Product>(data, statusCode, message);