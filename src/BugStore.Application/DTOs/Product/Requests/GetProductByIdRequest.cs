namespace BugStore.Application.DTOs.Product.Requests;

public class GetProductByIdRequest(Guid id){
    public Guid Id { get; set; } = id;
}