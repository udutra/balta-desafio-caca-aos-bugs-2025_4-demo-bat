using BugStore.Application.Interfaces;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses.Orders;

namespace BugStore.Application.Services;

public class OrderService : IOrderService{
    public Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<GetOrderByIdResponse> GetOrderByIdAsync(GetOrderByIdRequest request, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
}