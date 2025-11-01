using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Products;

public class UpdateProductResponse(Product? data, int statusCode = Configuration.DefaultStatusCode,
    string message = "Produto atualizado com sucesso.") : Response<Product>(data, statusCode, message);