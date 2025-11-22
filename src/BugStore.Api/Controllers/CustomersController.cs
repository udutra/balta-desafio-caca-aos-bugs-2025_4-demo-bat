using AutoMapper;
using BugStore.Application;
using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Customer;
using BugStore.Application.DTOs.Customer.Requests;
using BugStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService service, IMapper mapper) : ControllerBase{

    [ProducesResponseType(typeof(CustomerDto), 201)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 409)]
    [ProducesResponseType(typeof(Exception), 500)]
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest request, CancellationToken cancellationToken){
        var result = await service.CreateCustomerAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return MapResultToAction(result);

        var dto = mapper.Map<CustomerDto>(result.Data);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [ProducesResponseType(typeof(CustomerDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken cancellationToken){
        var result = await service.GetCustomerByIdAsync(new GetCustomerByIdRequest(id),
            cancellationToken);
        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<CustomerDto>(result.Data);
        return Ok(dto);
    }

    [ProducesResponseType(typeof(GetAllCustomersResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpGet]
    public async Task<ActionResult<GetAllCustomersResponseDto>> Get(CancellationToken cancellationToken,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize){

        var result = await service
            .GetAllCustomersAsync(new GetAllCustomersRequest(pageNumber, pageSize), cancellationToken);

        if (!result.IsSuccess) return MapResultToAction(result);

        var dtos = mapper.Map<IEnumerable<CustomerDto>>(result.Data);
        var responseDto = new GetAllCustomersResponseDto(dtos, result.TotalCount, pageNumber, pageSize,
            result.TotalPages, result.Code, result.Message);

        return Ok(responseDto);
    }

    [ProducesResponseType(typeof(CustomerDto), 200)]
    [ProducesResponseType(typeof(CustomerDto), 204)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, UpdateCustomerRequest request,
        CancellationToken cancellationToken){

        request.Id = id;
        var result = await service.UpdateCustomerAsync(request, cancellationToken);
        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<CustomerDto>(result.Data);
        return result.Code == 204 ? NoContent() : Ok(dto);
    }

    [ProducesResponseType(typeof(CustomerDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Delete(Guid id, CancellationToken cancellationToken){
        var result = await service.DeleteCustomerAsync(new DeleteCustomerRequest(id), cancellationToken);
        return !result.IsSuccess ? MapResultToAction(result) : NoContent();
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