namespace BugStore.Application.DTOs.Customer.Requests;

public class DeleteCustomerRequest(Guid id){
    public Guid Id { get; set; } = id;
}