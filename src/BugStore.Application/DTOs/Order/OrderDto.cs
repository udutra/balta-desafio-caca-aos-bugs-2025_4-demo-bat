using BugStore.Application.DTOs.Customer;
using BugStore.Application.DTOs.OrderLine;

namespace BugStore.Application.DTOs.Order;

public class OrderDto{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public CustomerDto Customer { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderLineDto> Lines { get; set; } = [];

    // Opcional para retornar status/mensagem junto com o DTO
    public int StatusCode { get; set; } = 200;
    public string? Message { get; set; } = "Sucesso";
}