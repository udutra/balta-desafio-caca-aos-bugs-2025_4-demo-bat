using BugStore.Domain.Exceptions;

namespace BugStore.Domain.Entities;

public class OrderLine{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private  set; }
    public int Quantity { get; private set; }
    public decimal Total { get; private set; }
    public Guid ProductId { get; private set; }
    public Product? Product{ get; private set; }
    public Order? Order { get; private set; }

    private OrderLine(){}

    public OrderLine(Guid orderId, int quantity, Product product){
        if (quantity <= 0)
            throw new DomainException("Quantidade tem que ser maior que zero.");

        if (product == null)
            throw new DomainException("Product não pode ser nullo");

        Id = Guid.CreateVersion7();
        OrderId = orderId;
        Quantity = quantity;
        Total = quantity * product.Price;
        ProductId = product.Id;
        Product = product;
    }
}