using AutoMapper;
using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Order;
using BugStore.Application.DTOs.Order.Requests;
using BugStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService service, IMapper mapper) : ControllerBase{

    [ProducesResponseType(typeof(OrderDto), 201)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request, CancellationToken cancellationToken){
        var result = await service.CreateOrderAsync(request, cancellationToken);

        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<OrderDto>(result.Data);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken){
        var request = new GetOrderByIdRequest(id);
        var result = await service.GetOrderByIdAsync(request, cancellationToken);

        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<OrderDto>(result.Data);
        return Ok(dto);
    }

    private ObjectResult MapResultToAction<T>(Response<T> result){
        return result.Code switch{
            400 => BadRequest(new ErrorDto(result.Code, result.Message)),
            404 => NotFound(new ErrorDto(result.Code, result.Message)),
            409 => Conflict(new ErrorDto(result.Code, result.Message)),
            _ => StatusCode(500, new ErrorDto(result.Code, result.Message))
        };
    }
}