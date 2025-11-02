namespace BugStore.Application.DTOs.Customer.Requests;

public class GetCustomerByIdRequest(Guid id){
    public Guid Id { get; set; } = id;
}