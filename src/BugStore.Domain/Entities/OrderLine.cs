namespace BugStore.Domain.Entities;

public class OrderLine{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product{ get; set; }
    public Order? Order { get; set; }

    private OrderLine(){}

    public OrderLine(Guid orderId, int quantity, Product product){
        if (quantity <= 0)
            throw new ArgumentException("Quantidade tem que ser maior que zero.", nameof(quantity));

        if (product == null)
            throw new ArgumentException("Product não pode ser nullo", nameof(product));

        Id = Guid.CreateVersion7();
        OrderId = orderId;
        Quantity = quantity;
        Total = quantity * product.Price;
        ProductId = product.Id;
        Product = product;
    }
}