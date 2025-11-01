using BugStore.Domain.Entities;

namespace BugStore.Application.Responses.Orders;

public class CreateOrderResponse(Order? data, int statusCode = 201, string message = "Pedido criado com sucesso.")
    : Response<Order>(data, statusCode, message);