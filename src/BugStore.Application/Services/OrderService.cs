using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Order.Requests;
using BugStore.Application.Interfaces;
using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Application.Services;

public class OrderService(IOrderRepository repository) : IOrderService{
    public async Task<Response<Order>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken){
        try{
            var createdOrder = await repository.CreateOrderAsync(new Order(request.Customer, request.Lines), cancellationToken);

            return new Response<Order>(createdOrder);
        }
        catch (DbUpdateException){
            return new Response<Order>(null, 500, "Erro ao criar o pedido. ErroCod: OS0001");
        }
        catch (OperationCanceledException){
            return new Response<Order>(null, 400, "Operação cancelada. ErroCod: OS0002");
        }
        catch (Exception){
            return new Response<Order>(null, 500, "Erro inesperado. ErroCod: OS0003");
        }
    }

    public async Task<Response<Order>> GetOrderByIdAsync(GetOrderByIdRequest request, CancellationToken cancellationToken){
        try{
            var order = await repository.GetOrderByIdAsync(request.Id, cancellationToken);

            return order is null ?
                new Response<Order>(null, 404, "Pedido não encontrado. ErroCod: OS0004")
                : new Response<Order>(order);
        }
        catch (OperationCanceledException){
            return new Response<Order>(null, 400, "Operação cancelada. ErroCod: OS0005");
        }
        catch (Exception){
            return new Response<Order>(null, 500,
                "Ocorreu um erro ao buscar o pedido. ErroCod: OS0006");
        }
    }
}