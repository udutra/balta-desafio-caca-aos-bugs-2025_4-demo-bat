using BugStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugStore.Infrastructure.Data.Mappings;

public class ProductMapping : IEntityTypeConfiguration<Product>{
    public void Configure(EntityTypeBuilder<Product> builder){
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasMaxLength(180);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasMaxLength(1000);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasMaxLength(180);

        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("DECIMAL(18,2)");

        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}