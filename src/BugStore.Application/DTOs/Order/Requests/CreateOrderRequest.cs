namespace BugStore.Application.DTOs.Order.Requests;

public class CreateOrderRequest(Guid customerId, Domain.Entities.Customer customer, List<Domain.Entities.OrderLine> lines){
    public Guid CustomerId { get; set; } = customerId;
    public Domain.Entities.Customer Customer { get; set; } = customer;
    public List<Domain.Entities.OrderLine> Lines { get; set; } = lines;
}