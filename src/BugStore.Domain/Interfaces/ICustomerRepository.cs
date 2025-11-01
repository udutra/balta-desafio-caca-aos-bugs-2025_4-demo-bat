using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken);
    Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken);
    Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken);
    Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken);
}