using BugStore.Application.Interfaces;
using BugStore.Domain.Entities;

namespace BugStore.Application.Services;

public class CustomerService : ICustomerService{
    public Task<(Customer? customer, string message)> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<(List<Customer> customers, bool success, string message)> GetAllCustomersAsync(CancellationToken cancellationToken){
        throw new NotImplementedException();
    }

    public Task<(Customer? customer, bool success, string message)> CreateCustomerAsync(string name, string email, string phone, DateTime birthDate,
        CancellationToken cancellationToken){
        throw new NotImplementedException();
    }

    public Task<(Customer? customer, bool success, string message)> UpdateCustomerAsync(Guid id, string? name, string? email, string? phone, DateTime? birthDate,
        CancellationToken cancellationToken){
        throw new NotImplementedException();
    }

    public Task<(bool success, string message)> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
}