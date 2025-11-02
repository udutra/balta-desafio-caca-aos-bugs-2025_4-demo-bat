namespace BugStore.Application.DTOs.Product.Requests;

public class DeleteProductRequest(Guid id){
    public Guid Id { get; set; } = id;
}