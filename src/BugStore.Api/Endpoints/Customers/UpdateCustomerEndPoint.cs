using BugStore.Api.Common.Api;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses.Customers;

namespace BugStore.Api.Endpoints.Customers;

public class UpdateCustomerEndPoint : IEndpoint{
    public static void Map(IEndpointRouteBuilder app) => app.MapPut("{id:guid}", HandleAsync)
        .WithName("Customer: Update")
        .WithSummary("Atualiza um cliente")
        .WithDescription("Atualiza um cliente")
        .WithOrder(4)
        .Produces<UpdateCustomerResponse>();

    private static async Task<IResult> HandleAsync(IHandlerCustomer handler, UpdateCustomerRequest request, Guid id){
        if (string.IsNullOrEmpty(id.ToString())){
            TypedResults.BadRequest(new UpdateCustomerResponse(null, 500, "Id do cliente é obrigatório"));
        }

        request.Id = id;

        var result = await handler.UpdateCustomerAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}