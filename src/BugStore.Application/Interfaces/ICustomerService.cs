using BugStore.Domain.Entities;

namespace BugStore.Application.Interfaces;

public interface ICustomerService{
    Task<(Customer? customer, string message)> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(List<Customer> customers, bool success, string message)> GetAllCustomersAsync(CancellationToken cancellationToken);
    Task<(Customer? customer, bool success, string message)> CreateCustomerAsync(
        string name, string email, string phone, DateTime birthDate, CancellationToken cancellationToken);
    Task<(Customer? customer, bool success, string message)> UpdateCustomerAsync(
        Guid id, string? name, string? email, string? phone, DateTime? birthDate, CancellationToken cancellationToken);
    Task<(bool success, string message)> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken);
}