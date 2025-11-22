using BugStore.Domain.Exceptions;

namespace BugStore.Domain.Entities;

public class Product{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }

    public Product(string title, string description, string slug, decimal price){
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O Título não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("O slug não pode ser vazio.");

        if (price <= 0){
            throw new DomainException("O preço deve ser maior que zero.");
        }

        Id = Guid.CreateVersion7();
        Title = title;
        Description = description;
        Slug = slug;
        Price = price;
    }
}