using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Customer.Requests;
using BugStore.Domain.Entities;

namespace BugStore.Application.Interfaces;

public interface ICustomerService{
    Task<Response<Customer>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken);
    Task<Response<Customer>> GetCustomerByIdAsync(GetCustomerByIdRequest request, CancellationToken cancellationToken);
    Task<PagedResponse<List<Customer>?>> GetAllCustomersAsync(GetAllCustomersRequest request, CancellationToken cancellationToken);
    Task<Response<Customer>> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken cancellationToken);
    Task<Response<Customer>> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken cancellationToken);
}