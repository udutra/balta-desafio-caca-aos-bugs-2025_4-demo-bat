namespace BugStore.Application.DTOs.Product.Requests;

public class CreateProductRequest(string title, string description, string slug, decimal price){
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public string Slug { get; set; } = slug;
    public decimal Price { get; set; } = price;
}