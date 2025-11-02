using AutoMapper;
using BugStore.Application;
using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Customers;
using BugStore.Application.Interfaces;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService service, IMapper mapper) : ControllerBase{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken){
        var result = await service.GetCustomerByIdAsync(new GetCustomerByIdRequest(id),
            cancellationToken);
        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<CustomerDto>(result.Data);
        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken,
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

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request, CancellationToken cancellationToken){
        var result = await service.CreateCustomerAsync(request, cancellationToken);

        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<CustomerDto>(result.Data);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request,
        CancellationToken cancellationToken){

        request.Id = id;
        var result = await service.UpdateCustomerAsync(request, cancellationToken);
        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<CustomerDto>(result.Data);
        return result.Code == 204 ? NoContent() : Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken){
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