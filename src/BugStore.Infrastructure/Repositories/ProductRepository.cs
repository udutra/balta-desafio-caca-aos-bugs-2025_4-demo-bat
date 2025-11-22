using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository {
    public async Task<Product?> CreateProductAsync(Product product, CancellationToken cancellationToken){
        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }
    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken){
        if (id == Guid.Empty)
            return null;

        return await context.Products.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Product?> GetProductBySlugAsync(string slug, CancellationToken cancellationToken){
        return string.IsNullOrWhiteSpace(slug)
            ? null
            : await context.Products.FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public IQueryable<Product> GetAllProducts(){
        return context.Products.AsNoTracking().AsQueryable();
    }
    public async Task<int> UpdateProductAsync(Product product, CancellationToken cancellationToken){
        var existing = await context.Products.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == product.Id, cancellationToken);

        if (existing is null)
            return 0;

        context.Products.Attach(product);

        if (existing.Title != product.Title)
            context.Entry(product).Property(c => c.Title).IsModified = true;

        if (existing.Description != product.Description)
            context.Entry(product).Property(c => c.Description).IsModified = true;

        if (existing.Slug != product.Slug)
            context.Entry(product).Property(c => c.Slug).IsModified = true;

        if (existing.Price != product.Price)
            context.Entry(product).Property(c => c.Price).IsModified = true;

        return await context.SaveChangesAsync(cancellationToken);
    }
    public async Task<int> DeleteProductAsync(Product product, CancellationToken cancellationToken){
        context.Products.Remove(product);
        return await context.SaveChangesAsync(cancellationToken);
    }
}