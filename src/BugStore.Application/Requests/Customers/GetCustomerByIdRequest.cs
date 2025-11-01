namespace BugStore.Application.Requests.Customers;

public class GetCustomerByIdRequest(Guid id){
    public Guid Id { get; set; } = id;
}