using BugStore.Application;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses.Customers;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(IHandlerCustomer handler) : ControllerBase{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(id.ToString())){
            BadRequest(new UpdateCustomerResponse(null, 500, "Id do cliente é obrigatório"));
        }
        var request = new GetCustomerByIdRequest(id);

        var result = await handler.GetCustomerByIdAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize){
        var request = new GetAllCustomersRequest(pageNumber, pageSize);

        var result = await handler.GetAllCustomersAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await handler.CreateCustomerAsync(request, cancellationToken);

        return response.IsSuccess
            ? Created($"/{response.Data?.Id}", response)
            : BadRequest(response.Data);
    }

    [HttpPost("Update")]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(id.ToString())){
            BadRequest(new UpdateCustomerResponse(null, 500, "Id do cliente é obrigatório"));
        }

        request.Id = id;

        var result = await handler.UpdateCustomerAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(id.ToString())){
            BadRequest(new UpdateCustomerResponse(null, 500, "Id do cliente é obrigatório"));
        }

        var request = new DeleteCustomerRequest(id);

        var result = await handler.DeleteCustomerAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}