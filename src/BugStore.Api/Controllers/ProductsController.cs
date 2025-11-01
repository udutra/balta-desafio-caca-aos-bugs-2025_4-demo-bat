using BugStore.Application;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Requests.Products;
using BugStore.Application.Responses.Products;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IHandlerProduct handler) : ControllerBase{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(id.ToString())){
            BadRequest(new GetProductByIdResponse(null, 500, "Id do produto é obrigatório"));
        }

        var request = new GetProductByIdRequest(id);

        var result = await handler.GetProductByIdAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize){
        var request = new GetAllProductsRequest(pageNumber, pageSize);

        var result = await handler.GetAllProductsAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken){
        var response = await handler.CreateProductAsync(request, cancellationToken);

        return response.IsSuccess
            ? Created($"/{response.Data?.Id}", response)
            : BadRequest(response.Data);
    }

    [HttpPost("Update")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(id.ToString())){
            BadRequest(new UpdateProductResponse(null, 500, "Id do produto é obrigatório"));
        }

        request.Id = id;

        var result = await handler.UpdateProductAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(id.ToString())){
            BadRequest(new UpdateProductResponse(null, 500, "Id do produto é obrigatório"));
        }

        var request = new DeleteProductRequest(id);

        var result = await handler.DeleteProductAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}