using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses.Orders;

namespace BugStore.Application.Interfaces;

public interface IOrderService{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    Task<GetOrderByIdResponse> GetOrderByIdAsync(GetOrderByIdRequest request, CancellationToken cancellationToken);
}