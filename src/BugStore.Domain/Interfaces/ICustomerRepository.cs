using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces;

public interface ICustomerRepository{
    Task<Customer?> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerByPhoneAsync(string email, CancellationToken cancellationToken);
    IQueryable<Customer> GetAllCustomers();
    Task<int> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken);
    Task<int> DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken);

}