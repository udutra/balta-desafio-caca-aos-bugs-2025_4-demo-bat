using BugStore.Api.Common.Api;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Products;
using BugStore.Application.Responses.Products;

namespace BugStore.Api.Endpoints.Products;

public class UpdateProductEndPoint : IEndpoint{
    public static void Map(IEndpointRouteBuilder app) => app.MapPut("{id:guid}", HandleAsync)
        .WithName("Product: Update")
        .WithSummary("Atualiza um produto")
        .WithDescription("Atualiza um ptoduto")
        .WithOrder(4)
        .Produces<UpdateProductResponse>();

    private static async Task<IResult> HandleAsync(IHandlerProduct handler, UpdateProductRequest request, Guid id){
        if (string.IsNullOrEmpty(id.ToString())){
            TypedResults.BadRequest(new UpdateProductResponse(null, 500, "Id do produto é obrigatório"));
        }

        request.Id = id;

        var result = await handler.UpdateProductAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}