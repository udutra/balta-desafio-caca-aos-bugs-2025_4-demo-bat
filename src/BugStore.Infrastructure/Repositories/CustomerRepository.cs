using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository{
    public async Task<Customer?> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken){
        await context.Customers.AddAsync(customer, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken){
        if (id == Guid.Empty)
            return null;

        return await context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken){
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<Customer?> GetCustomerByPhoneAsync(string phone, CancellationToken cancellationToken){
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        return await context.Customers
            .FirstOrDefaultAsync(c => c.Phone == phone, cancellationToken);
    }

    public IQueryable<Customer> GetAllCustomers(){
        return context.Customers.AsNoTracking().AsQueryable();
    }

    public async Task<int> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken){
        var existing = await context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customer.Id, cancellationToken);

        if (existing is null)
            return 0;

        // Atualiza apenas se algo mudou
        if (existing.Name != customer.Name)
            context.Entry(customer).Property(c => c.Name).IsModified = true;

        if (existing.Email != customer.Email)
            context.Entry(customer).Property(c => c.Email).IsModified = true;

        if (existing.Phone != customer.Phone)
            context.Entry(customer).Property(c => c.Phone).IsModified = true;

        if (existing.BirthDate != customer.BirthDate)
            context.Entry(customer).Property(c => c.BirthDate).IsModified = true;

        context.Customers.Attach(customer);
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken){
        context.Customers.Remove(customer);
        return await context.SaveChangesAsync(cancellationToken);
    }
}