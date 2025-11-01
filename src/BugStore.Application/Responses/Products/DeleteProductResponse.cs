using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Products;

public class DeleteProductResponse(Product? data, int statusCode = Configuration.DefaultStatusCode,
    string message = "Produto removido com sucesso.")
    : Response<Product>(data, statusCode, message);