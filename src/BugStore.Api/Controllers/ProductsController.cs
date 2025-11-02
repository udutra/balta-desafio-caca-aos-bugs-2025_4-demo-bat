using AutoMapper;
using BugStore.Application;
using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Product;
using BugStore.Application.DTOs.Product.Requests;
using BugStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service, IMapper mapper) : ControllerBase{

    [ProducesResponseType(typeof(ProductDto), 201)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 409)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken cancellationToken){
        var result = await service.CreateProductAsync(request, cancellationToken);

        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<ProductDto>(result.Data);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken){
        var result = await service.GetProductByIdAsync(new GetProductByIdRequest(id),
            cancellationToken);
        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<ProductDto>(result.Data);
        return Ok(dto);
    }

    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpGet]
    public async Task<ActionResult<GetAllProductsResponseDto>> Get(CancellationToken cancellationToken,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize){

        var result = await service
            .GetAllProductsAsync(new GetAllProductsRequest(pageNumber, pageSize), cancellationToken);

        if (!result.IsSuccess) return MapResultToAction(result);

        var dtos = mapper.Map<IEnumerable<ProductDto>>(result.Data);
        var responseDto = new GetAllProductsResponseDto(dtos, result.TotalCount, pageNumber, pageSize, result.TotalPages,
            result.Code, result.Message);

        return Ok(responseDto);
    }

    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(ProductDto), 204)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, UpdateProductRequest request, CancellationToken cancellationToken){
        request.Id = id;
        var result = await service.UpdateProductAsync(request, cancellationToken);
        if (!result.IsSuccess) return MapResultToAction(result);

        var dto = mapper.Map<ProductDto>(result.Data);
        return result.Code == 204 ? NoContent() : Ok(dto);
    }

    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 404)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Delete(Guid id, CancellationToken cancellationToken){
        var result = await service.DeleteProductAsync(new DeleteProductRequest(id), cancellationToken);
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