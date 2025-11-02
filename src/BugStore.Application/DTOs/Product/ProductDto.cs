namespace BugStore.Application.DTOs.Product;

public class ProductDto{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public decimal Price { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}