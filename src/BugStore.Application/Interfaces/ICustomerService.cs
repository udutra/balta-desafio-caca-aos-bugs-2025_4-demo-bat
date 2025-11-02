using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses.Customers;
namespace BugStore.Application.Interfaces;

public interface ICustomerService{
    Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken);
    Task<GetCustomerByIdResponse> GetCustomerByIdAsync(GetCustomerByIdRequest request, CancellationToken cancellationToken);
    Task<GetAllCustomersResponse> GetAllCustomersAsync(GetAllCustomersRequest request, CancellationToken cancellationToken);
    Task<UpdateCustomerResponse> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken cancellationToken);
    Task<DeleteCustomerResponse> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken cancellationToken);
}