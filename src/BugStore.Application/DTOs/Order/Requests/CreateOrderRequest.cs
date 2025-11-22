using BugStore.Application.DTOs.OrderLine;

namespace BugStore.Application.DTOs.Order.Requests;

public class CreateOrderRequest(Guid customerId, List<OrderLineDto> lines){
    public Guid CustomerId{ get; set; } = customerId;
    public List<OrderLineDto> Lines { get; set; } = lines;
}