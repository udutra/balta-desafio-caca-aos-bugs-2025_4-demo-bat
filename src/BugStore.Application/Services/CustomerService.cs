using BugStore.Application.Interfaces;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses.Customers;
using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Application.Services;

public class CustomerService(ICustomerRepository repository) : ICustomerService{

    public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request,
        CancellationToken cancellationToken){
        try{
            var exists = await repository.GetCustomerByEmailAsync(request.Email, cancellationToken)
                         ?? await repository.GetCustomerByPhoneAsync(request.Phone, cancellationToken);

            if (exists is not null)
                return new CreateCustomerResponse(null, 409, "Cliente já cadastrado. ErroCod: CS0001");

            var customerEntity = new Customer(request.Name, request.Email, request.Phone, request.BirthDate);
            var createdCustomer = await repository.CreateCustomerAsync(customerEntity, cancellationToken);

            return new CreateCustomerResponse(createdCustomer);
        }
        catch (DbUpdateException){
            return new CreateCustomerResponse(null, 500, "Erro ao criar o cliente. ErroCod: CS0002");
        }
        catch (Exception){
            return new CreateCustomerResponse(null, 500, "Erro inesperado. ErroCod: CS0003");
        }
    }

    public async Task<GetCustomerByIdResponse> GetCustomerByIdAsync(GetCustomerByIdRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new GetCustomerByIdResponse(null, 400, "Id informado inválido. ErroCod: CS0004");

            var customer = await repository.GetCustomerByIdAsync(request.Id, cancellationToken);

            return customer == null ? new GetCustomerByIdResponse(null, 404,
                "Cliente com Id informado não encontrado. ErroCod: CS0005") : new GetCustomerByIdResponse(customer);
        }
        catch (OperationCanceledException){
            return new GetCustomerByIdResponse(null, 400, "Operação cancelada. ErroCod: CS0006");
        }
        catch{
            return new GetCustomerByIdResponse(null, 500,
                "Ocorreu um erro ao buscar o cliente. ErroCod: CS0007");
        }
    }

    public async Task<GetAllCustomersResponse> GetAllCustomersAsync(GetAllCustomersRequest request, CancellationToken cancellationToken){
        try{
            if (request.PageNumber < 1 || request.PageSize <= 0){
                return new GetAllCustomersResponse(null,-1, -1, -1, 400,
                    "Parâmetros de paginação inválidos. ErroCod: CS0008");
            }

            var query  = repository.GetAllCustomers();

            var total = query.CountAsync(cancellationToken: cancellationToken);

            var customers = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            await Task.WhenAll(total, customers);

            return new GetAllCustomersResponse(customers.Result, total.Result, request.PageNumber, request.PageSize,
                200, total.Result == 0 ? "Nenhum cliente cadastrado." :
                    "Lista de clientes retornada com sucesso.");
        }
        catch (OperationCanceledException){
            return new GetAllCustomersResponse(null,-1, -1, -1, 499,
                "Operação cancelada. ErroCod: CS0009");
        }
        catch{
            return new GetAllCustomersResponse(null,-1, -1, -1, 500,
                "Ocorreu um erro ao recuperar os clientes. ErroCod: CS0010");
        }
    }

    public async Task<UpdateCustomerResponse> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new UpdateCustomerResponse(null, 400, "Id informado inválido. ErroCod: CS0011");

            var customer = await repository.GetCustomerByIdAsync(request.Id, cancellationToken);

            if (customer is null){
                return new UpdateCustomerResponse(null, 404, "Cliente não encontrado. ErroCod: CS0012");
            }

            customer.Update(request.Name, request.Email, request.Phone, request.BirthDate);
            var response = await repository.UpdateCustomerAsync(customer, cancellationToken);

            return response == 0
                ? new UpdateCustomerResponse(null, 204, "Cliente não modificado. ErroCod: CS0013")
                : new UpdateCustomerResponse(customer);
        }
        catch (NotSupportedException){
            return new UpdateCustomerResponse(null, 500,
                "Erro ao atualizar o cliente. ErroCod: CS0014");
        }
        catch (DbUpdateException){
            return new UpdateCustomerResponse(null, 500,
                "Erro ao atualizar o cliente. ErroCod: CS0015");
        }
        catch (OperationCanceledException){
            return new UpdateCustomerResponse(null, 400, "Operação cancelada. ErroCod: CS0016");
        }
        catch (Exception){
            return new UpdateCustomerResponse(null, 500, "Erro inesperado. ErroCod: CS0017");
        }
    }

    public async Task<DeleteCustomerResponse> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new DeleteCustomerResponse(null, 400, "Id informado inválido. ErroCod: CS0018");

            var customer = await repository.GetCustomerByIdAsync(request.Id, cancellationToken);
            if (customer is null)
                return new DeleteCustomerResponse(null, 404, "Cliente não encontrado. ErroCod: CS0019");


            var response = await repository.DeleteCustomerAsync(customer, cancellationToken);

            return response == 0 ?
                new DeleteCustomerResponse(null, 400, "Cliente não removido. ErroCod: CS0020") :
                new DeleteCustomerResponse(customer);
        }
        catch (NotSupportedException){
            return new DeleteCustomerResponse(null, 500,
                "Erro ao remover o cliente. ErroCod: CS0021");
        }
        catch (DbUpdateException){
            return new DeleteCustomerResponse(null, 500,
                $"Erro ao remover o cliente. ErroCod: CS0022");
        }
        catch (OperationCanceledException){
            return new DeleteCustomerResponse(null, 400, "Operação cancelada. ErroCod: CS0023");
        }
        catch (Exception){
            return new DeleteCustomerResponse(null, 500,
                "Erro inesperado. ErroCod: CS0024");
        }
    }
}