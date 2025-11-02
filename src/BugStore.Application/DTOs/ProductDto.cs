namespace BugStore.Application.DTOs;

public record ProductDto(Guid Id, string Title, string Description, string Slug, decimal Price);