using BugStore.Domain.Exceptions;

namespace BugStore.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderLine> Lines { get; set; } = [];

    private Order() { }

    public Order(Customer customer, List<OrderLine> lines){
        if (customer == null)
            throw new DomainException("Cliente não pode ser nullo");

        if (lines.Count == 0){
            throw new DomainException("Lista de produtos deve ter no mínimo 1 item.");
        }

        Id = Guid.CreateVersion7();
        CustomerId = customer.Id;
        Customer = customer;
        Lines = lines;
        CreatedAt = DateTime.UtcNow;
    }
}