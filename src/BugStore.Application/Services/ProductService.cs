using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Product.Requests;
using BugStore.Application.Interfaces;
using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Application.Services;

public class ProductService(IProductRepository repository) : IProductService {
    public async Task<Response<Product>> CreateProductAsync(CreateProductRequest request,
        CancellationToken cancellationToken){
        try{
            var exists = await repository.GetProductBySlugAsync(request.Slug, cancellationToken);

            if (exists is not null)
                return new Response<Product>(null, 409,
                    "O slug informado já está em uso. ErroCod: PS0001");

            var productEntity = new Product(request.Title, request.Description, request.Slug, request.Price);
            var createdProduct = await repository.CreateProductAsync(productEntity, cancellationToken);

            return new Response<Product>(createdProduct);
        }
        catch (DbUpdateException){
            return new Response<Product>(null, 500, "Erro ao criar o produto. ErroCod: PS0002");
        }
        catch (OperationCanceledException){
            return new Response<Product>(null, 400, "Operação cancelada. ErroCod: PS0003");
        }
        catch (Exception){
            return new Response<Product>(null, 500, "Erro inesperado. ErroCod: PS0004");
        }
    }

    public async Task<Response<Product>> GetProductByIdAsync(GetProductByIdRequest request,
        CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new Response<Product>(null, 400, "Id informado inválido. ErroCod: PS0005");

            var product = await repository.GetProductByIdAsync(request.Id, cancellationToken);

            return product == null ? new Response<Product>(null, 404,
                "Produto com Id informado não encontrado. ErroCod: PS0006") :
                new Response<Product>(product);
        }
        catch (OperationCanceledException){
            return new Response<Product>(null, 400, "Operação cancelada. ErroCod: PS0007");
        }
        catch{
            return new Response<Product>(null, 500,
                "Ocorreu um erro ao buscar o produto. ErroCod: PS0008");
        }
    }

    public async Task<PagedResponse<List<Product>?>> GetAllProductsAsync(GetAllProductsRequest request,
        CancellationToken cancellationToken){
        try{
            if (request.PageNumber < 1 || request.PageSize <= 0){
                return new PagedResponse<List<Product>?>(null, -1, 400,
                    request.PageNumber, request.PageSize,
                    "Parâmetros de paginação inválidos. ErroCod: PS0009");
            }

            var query  = repository.GetAllProducts();

            var total = await query.CountAsync(cancellationToken);
            var products = await query.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<List<Product>?>(products, total, 200,
                request.PageNumber, request.PageSize, total == 0 ? "Nenhum produto cadastrado." :
                    "Lista de produtos retornada com sucesso.");
        }
        catch (OperationCanceledException){
            return new PagedResponse<List<Product>?>(null,-1, -1, -1, 400,
                "Operação cancelada. ErroCod: PS0010");
        }
        catch{
            return new PagedResponse<List<Product>?>(null,-1, -1, -1, 500,
                "Ocorreu um erro ao recuperar os produtos. ErroCod: PS0011");
        }
    }
    public async Task<Response<Product>> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new Response<Product>(null, 400, "Id informado inválido. ErroCod: PS0012");

            var product = await repository.GetProductByIdAsync(request.Id, cancellationToken);

            if (product is null){
                return new Response<Product>(null, 404, "Produto não encontrado. ErroCod: PS0013");
            }

            var response = await repository.UpdateProductAsync(product, cancellationToken);

            return response == 0
                ? new Response<Product>(null, 204, "Produto não modificado. ErroCod: PS0014")
                : new Response<Product>(product);
        }
        catch (NotSupportedException){
            return new Response<Product>(null, 500,
                "Erro ao atualizar o produto. ErroCod: PS0015");
        }
        catch (DbUpdateException){
            return new Response<Product>(null, 500,
                "Erro ao atualizar o produto. ErroCod: PS0016");
        }
        catch (OperationCanceledException){
            return new Response<Product>(null, 400, "Operação cancelada. ErroCod: PS0017");
        }
        catch (Exception){
            return new Response<Product>(null, 500, "Erro inesperado. ErroCod: PS0018");
        }
    }
    public async Task<Response<Product>> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken){
        try{
            if (request.Id == Guid.Empty)
                return new Response<Product>(null, 400, "Id informado inválido. ErroCod: CS0019");

            var product = await repository.GetProductByIdAsync(request.Id, cancellationToken);
            if (product is null)
                return new Response<Product>(null, 404, "Produto não encontrado. ErroCod: CS0020");


            var response = await repository.DeleteProductAsync(product, cancellationToken);

            return response == 0 ?
                new Response<Product>(null, 400, "Produto não removido. ErroCod: CS0021") :
                new Response<Product>(product);
        }
        catch (NotSupportedException){
            return new Response<Product>(null, 500, "Erro ao remover o produto. ErroCod: CS0022");
        }
        catch (DbUpdateException){
            return new Response<Product>(null, 500, $"Erro ao remover o produto. ErroCod: CS0023");
        }
        catch (OperationCanceledException){
            return new Response<Product>(null, 400, "Operação cancelada. ErroCod: CS0024");
        }
        catch (Exception){
            return new Response<Product>(null, 500, "Erro inesperado. ErroCod: CS0025");
        }
    }
}