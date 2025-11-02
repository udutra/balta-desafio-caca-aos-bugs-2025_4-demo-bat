namespace BugStore.Application.DTOs.OrderLine;

public class OrderLineDto{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}