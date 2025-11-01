using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses.Orders;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Application.Handlers.Orders;

public class OrderHandler(AppDbContext context) : IHandlerOrder{

    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request,
        CancellationToken cancellationToken = default){
        try{
            var order = new Order(request.Customer, request.Lines);
            await context.Orders.AddAsync(order, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateOrderResponse(order);
        }
        catch (DbUpdateException){
            return new CreateOrderResponse(null, 500,
                "Erro ao criar o pedido. ErroCod: OH0001");
        }
        catch (OperationCanceledException){
            return new CreateOrderResponse(null, 499, "Operação cancelada.");
        }
        catch (Exception){
            return new CreateOrderResponse(null, 500,
                "Erro inesperado. ErroCod: OH0001-GEN");
        }
    }

    public async Task<GetOrderByIdResponse> GetOrderByIdAsync(GetOrderByIdRequest request,
        CancellationToken cancellationToken = default){
        try{
            var order = await context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            return order is null ?
                new GetOrderByIdResponse(null, 404, "Pedido não encontrado.")
                : new GetOrderByIdResponse(order);
        }
        catch (OperationCanceledException){
            return new GetOrderByIdResponse(null, 499, "Operação cancelada.");
        }
        catch{
            return new GetOrderByIdResponse(null, 500,
                "Ocorreu um erro ao buscar o pedido. ErroCod: OH0002");
        }
    }
}