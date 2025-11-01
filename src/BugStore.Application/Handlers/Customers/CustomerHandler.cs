using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses.Customers;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Application.Handlers.Customers;

public class CustomerHandler(AppDbContext context) : IHandlerCustomer{

    public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request,
        CancellationToken cancellationToken = default){
        try{
            if (await context.Customers.AnyAsync(c => c.Email == request.Email, cancellationToken))
                return new CreateCustomerResponse(null, 409,
                    "Já existe um cliente com esse e-mail.");

            var customer = new Customer(request.Name, request.Email, request.Phone, request.BirthDate);
            await context.Customers.AddAsync(customer, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateCustomerResponse(customer);
        }
        catch (DbUpdateException){
            return new CreateCustomerResponse(null, 500,
                "Erro ao criar o cliente. ErroCod: CH0001");
        }
        catch (Exception){
            return new CreateCustomerResponse(null, 500,
                "Erro inesperado. ErroCod: CH0001-GEN");
        }
    }

    public async Task<GetCustomerByIdResponse> GetCustomerByIdAsync(GetCustomerByIdRequest request,
        CancellationToken cancellationToken = default){
        try{
            var customer = await context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            return customer is null ?
                new GetCustomerByIdResponse(null, 404, "Cliente não encontrado.")
                : new GetCustomerByIdResponse(customer);
        }
        catch (OperationCanceledException){
            return new GetCustomerByIdResponse(null, 499, "Operação cancelada.");
        }
        catch{
            return new GetCustomerByIdResponse(null, 500,
                "Ocorreu um erro ao buscar o cliente. ErroCod: CH0002");
        }
    }

    public async Task<GetAllCustomersResponse> GetAllCustomersAsync(GetAllCustomersRequest request,
        CancellationToken cancellationToken = default){
        try{
            var query  = context.Customers.AsNoTracking().OrderBy(c => c.Name);

            var total = query.CountAsync(cancellationToken);

            var customers = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            await Task.WhenAll(total, customers);

            return new GetAllCustomersResponse(customers.Result, total.Result, request.PageNumber, request.PageSize);
        }
        catch (OperationCanceledException){
            return new GetAllCustomersResponse(null, 499, "Operação cancelada.");
        }
        catch{
            return new GetAllCustomersResponse(null, 500,
                "Ocorreu um erro ao recuperar os clientes. ErroCod: CH0003");
        }
    }

    public async Task<UpdateCustomerResponse> UpdateCustomerAsync(UpdateCustomerRequest request,
        CancellationToken cancellationToken = default){
        try{
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (customer is null){
                return new UpdateCustomerResponse(null, 404, "Cliente não encontrado.");
            }

            customer.Update(request.Name, request.Email, request.Phone, request.BirthDate);
            
            context.Customers.Update(customer);
            await context.SaveChangesAsync(cancellationToken);

            return new UpdateCustomerResponse(customer);
        }
        catch (DbUpdateException){
            return new UpdateCustomerResponse(null, 500,
                "Erro ao atualizar o cliente. ErroCod: CH0004");
        }
        catch (OperationCanceledException){
            return new UpdateCustomerResponse(null, 499, "Operação cancelada.");
        }
        catch (Exception)
        {
            return new UpdateCustomerResponse(null, 500,
                "Erro inesperado. ErroCod: CH0004-GEN");
        }
    }

    public async Task<DeleteCustomerResponse> DeleteCustomerAsync(DeleteCustomerRequest request,
        CancellationToken cancellationToken = default){
        try{
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (customer is null)
                return new DeleteCustomerResponse(null, 404, "Cliente não encontrado.");

            context.Customers.Remove(customer);
            await context.SaveChangesAsync(cancellationToken);

            return new DeleteCustomerResponse();
        }
        catch (DbUpdateException){
            return new DeleteCustomerResponse(null, 500,
                $"Erro ao remover o cliente. ErroCod: CH0005");
        }
        catch (OperationCanceledException){
            return new DeleteCustomerResponse(null, 499, "Operação cancelada.");
        }
        catch (Exception){
            return new DeleteCustomerResponse(null, 500,
                "Erro inesperado. ErroCod: CH0005-GEN");
        }
    }
}