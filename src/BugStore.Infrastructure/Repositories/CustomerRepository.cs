using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;

namespace BugStore.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository{
    public Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
    public Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken){
        throw new NotImplementedException();
    }
}