using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Order.Requests;
using BugStore.Application.Interfaces;
using BugStore.Domain.Entities;
using BugStore.Domain.Exceptions;
using BugStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Application.Services;

public class OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository,
    IProductRepository productRepository) : IOrderService{
    public async Task<Response<Order>> CreateOrderAsync(CreateOrderRequest request,
        CancellationToken cancellationToken){

        try{
            var customer = await customerRepository.GetCustomerByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
                return new Response<Order>(null, 404, "Cliente não encontrado. ErroCod: OS0001");

            var lines = new List<OrderLine>();
            foreach (var lineDto in request.Lines)
            {
                var product = await productRepository.GetProductByIdAsync(lineDto.ProductId, cancellationToken);
                if (product is null)
                    throw new DomainException($"Produto {lineDto.ProductId} não encontrado. ErroCod: OS0003");

                lines.Add(new OrderLine(Guid.Empty, lineDto.Quantity, product));
            }

            var order = new Order(customer, lines);
            var createdOrder = await orderRepository.CreateOrderAsync(order, cancellationToken);

            return new Response<Order>(createdOrder, 201, "Pedido criado com sucesso.");
        }
        catch (DomainException ex){
            return new Response<Order>(null, 400, ex.Message);
        }
        catch (DbUpdateException){
            return new Response<Order>(null, 500, "Erro ao criar o pedido. ErroCod: OS0004");
        }
        catch (OperationCanceledException){
            return new Response<Order>(null, 400, "Operação cancelada. ErroCod: OS0005");
        }
        catch (Exception){
            return new Response<Order>(null, 500, "Erro inesperado. ErroCod: OS0006");
        }
    }

    public async Task<Response<Order>> GetOrderByIdAsync(GetOrderByIdRequest request, CancellationToken cancellationToken){
        try{
            var order = await orderRepository.GetOrderByIdAsync(request.Id, cancellationToken);

            return order is null ?
                new Response<Order>(null, 404, "Pedido não encontrado. ErroCod: OS0007")
                : new Response<Order>(order);
        }
        catch (OperationCanceledException){
            return new Response<Order>(null, 400, "Operação cancelada. ErroCod: OS0008");
        }
        catch (Exception){
            return new Response<Order>(null, 500,
                "Ocorreu um erro ao buscar o pedido. ErroCod: OS0009");
        }
    }
}