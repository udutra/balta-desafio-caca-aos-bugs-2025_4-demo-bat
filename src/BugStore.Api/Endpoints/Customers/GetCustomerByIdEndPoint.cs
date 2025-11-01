using BugStore.Api.Common.Api;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses.Customers;

namespace BugStore.Api.Endpoints.Customers;

public class GetCustomerByIdEndPoint : IEndpoint{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("{id:guid}", HandleAsync)
            .WithName("Customer: Get by Id")
            .WithSummary("Busca o cliente pelo Id")
            .WithDescription("Busca o cliente pelo Id")
            .WithOrder(2)
            .Produces<GetCustomerByIdResponse>();

    private static async Task<IResult> HandleAsync(IHandlerCustomer handler, Guid id){
        if (string.IsNullOrEmpty(id.ToString())){
            TypedResults.BadRequest(new UpdateCustomerResponse(null, 500, "Id do cliente é obrigatório"));
        }
        var request = new GetCustomerByIdRequest(id);

        var result = await handler.GetCustomerByIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}