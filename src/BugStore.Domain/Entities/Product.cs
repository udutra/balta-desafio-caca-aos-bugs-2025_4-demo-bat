namespace BugStore.Domain.Entities;

public class Product{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }

    public Product(string title, string description, string slug, decimal price){
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("O Título não pode ser vazio.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição não pode ser vazio.", nameof(description));

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("O slug não pode ser vazio.", nameof(slug));

        if (price <= 0){
            throw new ArgumentException("O preço deve ser maior que zero.", nameof(price));
        }

        Id = Guid.CreateVersion7();
        Title = title;
        Description = description;
        Slug = slug;
        Price = price;
    }
}