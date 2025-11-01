using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Orders;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IHandlerOrder handler) : ControllerBase{

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken){
        var response = await handler.CreateOrderAsync(request, cancellationToken);

        return response.IsSuccess
            ? Created($"/{response.Data?.Id}", response)
            : BadRequest(response.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken){
        var request = new GetOrderByIdRequest(id);

        var result = await handler.GetOrderByIdAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}