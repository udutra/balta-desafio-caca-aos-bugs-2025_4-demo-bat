namespace BugStore.Application.Requests.Products;

public class DeleteProductRequest(Guid id){
    public Guid Id { get; set; } = id;
}