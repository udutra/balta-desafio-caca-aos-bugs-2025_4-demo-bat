using BugStore.Api.Common.Api;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses.Orders;

namespace BugStore.Api.Endpoints.Orders;

public class GetOrderByIdEndPoint : IEndpoint{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{id:guid}", HandleAsync)
            .WithName("Order: Get by Id")
            .WithSummary("Busca um pedido pelo Id")
            .WithDescription("Busca o pedido pelo Id")
            .WithOrder(2)
            .Produces<GetOrderByIdResponse>();

    private static async Task<IResult> HandleAsync(IHandlerOrder handler, Guid id){
        var request = new GetOrderByIdRequest(id);

        var result = await handler.GetOrderByIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}