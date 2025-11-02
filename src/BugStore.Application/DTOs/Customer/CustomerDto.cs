namespace BugStore.Application.DTOs.Customer;

public class CustomerDto{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}