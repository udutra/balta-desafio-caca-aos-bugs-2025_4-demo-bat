namespace BugStore.Application.DTOs.Order.Requests;

public class GetOrderByIdRequest(Guid id){
    public Guid Id { get; set; } = id;
}