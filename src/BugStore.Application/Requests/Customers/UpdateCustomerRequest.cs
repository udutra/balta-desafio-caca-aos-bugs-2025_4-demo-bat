namespace BugStore.Application.Requests.Customers;

public class UpdateCustomerRequest(string? name, string? email, string? phone, DateTime? birthDate){
    public Guid Id { get; set; }
    public string? Name { get; set; } = name;
    public string? Email { get; set; } = email;
    public string? Phone { get; set; } = phone;
    public DateTime? BirthDate { get; set; } = birthDate;
}