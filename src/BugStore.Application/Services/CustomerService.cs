using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Customer.Requests;
using BugStore.Application.Interfaces;
using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Application.Services;

public class CustomerService(ICustomerRepository repository) : ICustomerService{

    public async Task<Response<Customer>> CreateCustomerAsync(CreateCustomerRequest request,
        CancellationToken cancellationToken){
        try{
            var exists = await repository.GetCustomerByEmailAsync(request.Email, cancellationToken)
                         ?? await repository.GetCustomerByPhoneAsync(request.Phone, cancellationToken);

            if (exists is not null)
                return new Response<Customer>(null, 409, "Cliente já cadastrado. ErroCod: CS0001");

            var customerEntity = new Customer(request.Name, request.Email, request.Phone, request.BirthDate);
            var createdCustomer = await repository.CreateCustomerAsync(customerEntity, cancellationToken);

            return new Response<Customer>(createdCustomer);
        }
        catch (DbUpdateException){
            return new Response<Customer>(null, 500, "Erro ao criar o cliente. ErroCod: CS0003");
        }
        catch (OperationCanceledException){
            return new Response<Customer>(null, 400, "Operação cancelada. ErroCod: CS00002");
        }
    }

    public async Task<Response<Customer>> GetCustomerByIdAsync(GetCustomerByIdRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new Response<Customer>(null, 400, "Id informado inválido. ErroCod: CS0005");

            var customer = await repository.GetCustomerByIdAsync(request.Id, cancellationToken);

            return customer == null ? new Response<Customer>(null, 404,
                "Cliente com Id informado não encontrado. ErroCod: CS0006") : new Response<Customer>(customer);
        }
        catch (OperationCanceledException){
            return new Response<Customer>(null, 400, "Operação cancelada. ErroCod: CS0007");
        }
        catch{
            return new Response<Customer>(null, 500,
                "Ocorreu um erro ao buscar o cliente. ErroCod: CS0008");
        }
    }

    public async Task<PagedResponse<List<Customer>?>> GetAllCustomersAsync(GetAllCustomersRequest request, CancellationToken cancellationToken){
        try{
            if (request.PageNumber < 1 || request.PageSize <= 0){
                return new PagedResponse<List<Customer>?>(null,-1, -1, -1, 400,
                    "Parâmetros de paginação inválidos. ErroCod: CS0009");
            }

            var query  = repository.GetAllCustomers();

            var total = await query.CountAsync(cancellationToken);
            var customers = await query.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<List<Customer>?>(customers, total, request.PageNumber, request.PageSize,
                200, total == 0 ? "Nenhum cliente cadastrado." : "Lista de clientes retornada com sucesso.");
        }
        catch (OperationCanceledException){
            return new PagedResponse<List<Customer>?>(null,-1, -1, -1, 499,
                "Operação cancelada. ErroCod: CS0010");
        }
        catch{
            return new PagedResponse<List<Customer>?>(null,-1, -1, -1, 500,
                "Ocorreu um erro ao recuperar os clientes. ErroCod: CS0011");
        }
    }

    public async Task<Response<Customer>> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new Response<Customer>(null, 400, "Id informado inválido. ErroCod: CS0012");

            var customer = await repository.GetCustomerByIdAsync(request.Id, cancellationToken);

            if (customer is null){
                return new Response<Customer>(null, 404, "Cliente não encontrado. ErroCod: CS0013");
            }

            var response = await repository.UpdateCustomerAsync(customer, cancellationToken);

            return response == 0
                ? new Response<Customer>(null, 204, "Cliente não modificado. ErroCod: CS0014")
                : new Response<Customer>(customer);
        }
        catch (NotSupportedException){
            return new Response<Customer>(null, 500, "Erro ao atualizar o cliente. ErroCod: CS0015");
        }
        catch (DbUpdateException){
            return new Response<Customer>(null, 500, "Erro ao atualizar o cliente. ErroCod: CS0016");
        }
        catch (OperationCanceledException){
            return new Response<Customer>(null, 400, "Operação cancelada. ErroCod: CS0017");
        }
        catch (Exception){
            return new Response<Customer>(null, 500, "Erro inesperado. ErroCod: CS0018");
        }
    }

    public async Task<Response<Customer>> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new Response<Customer>(null, 400, "Id informado inválido. ErroCod: CS0019");

            var customer = await repository.GetCustomerByIdAsync(request.Id, cancellationToken);
            if (customer is null)
                return new Response<Customer>(null, 404, "Cliente não encontrado. ErroCod: CS0020");


            var response = await repository.DeleteCustomerAsync(customer, cancellationToken);

            return response == 0 ?
                new Response<Customer>(null, 400, "Cliente não removido. ErroCod: CS0021") :
                new Response<Customer>(customer);
        }
        catch (NotSupportedException){
            return new Response<Customer>(null, 500, "Erro ao remover o cliente. ErroCod: CS0022");
        }
        catch (DbUpdateException){
            return new Response<Customer>(null, 500, $"Erro ao remover o cliente. ErroCod: CS0023");
        }
        catch (OperationCanceledException){
            return new Response<Customer>(null, 400, "Operação cancelada. ErroCod: CS0024");
        }
        catch (Exception){
            return new Response<Customer>(null, 500, "Erro inesperado. ErroCod: CS0025");
        }
    }
}