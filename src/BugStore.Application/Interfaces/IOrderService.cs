using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Order.Requests;
using BugStore.Domain.Entities;

namespace BugStore.Application.Interfaces;

public interface IOrderService{
    Task<Response<Order>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    Task<Response<Order>> GetOrderByIdAsync(GetOrderByIdRequest request, CancellationToken cancellationToken);
}