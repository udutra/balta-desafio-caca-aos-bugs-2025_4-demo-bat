using BugStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugStore.Infrastructure.Data.Mappings;

public class CustomerMapping : IEntityTypeConfiguration<Customer>{
    public void Configure(EntityTypeBuilder<Customer> builder){
        builder.ToTable("Customers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasMaxLength(150);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasColumnType("TEXT")
            .HasMaxLength(11);
        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Phone).IsUnique();
    }
}