using BugStore.Api.Common.Api;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses.Customers;

namespace BugStore.Api.Endpoints.Customers;

public class DeleteCustomerEndPoint : IEndpoint{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
        .WithName("Customers: Delete")
        .WithSummary("Exclui um cliente")
        .WithDescription("Exclui um cliente")
        .WithOrder(5)
        .Produces<DeleteCustomerResponse>();

    private static async Task<IResult> HandleAsync(IHandlerCustomer handler, Guid id){
        if (string.IsNullOrEmpty(id.ToString())){
            TypedResults.BadRequest(new UpdateCustomerResponse(null, 500, "Id do cliente é obrigatório"));
        }

        var request = new DeleteCustomerRequest(id);

        var result = await handler.DeleteCustomerAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}