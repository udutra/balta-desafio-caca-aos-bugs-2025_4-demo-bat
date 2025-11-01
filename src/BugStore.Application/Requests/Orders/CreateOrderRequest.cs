using BugStore.Domain.Entities;

namespace BugStore.Application.Requests.Orders;

public class CreateOrderRequest(Guid customerId, Customer customer, List<OrderLine> lines){
    public Guid CustomerId { get; set; } = customerId;
    public Customer Customer { get; set; } = customer;
    public List<OrderLine> Lines { get; set; } = lines;
}