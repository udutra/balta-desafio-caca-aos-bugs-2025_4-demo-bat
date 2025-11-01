using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses.Orders;

namespace BugStore.Application.Handlers.Interfaces;

public interface IHandlerOrder{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request,
        CancellationToken cancellationToken = default);
    Task<GetOrderByIdResponse> GetOrderByIdAsync(GetOrderByIdRequest request,
        CancellationToken cancellationToken = default);
}