using BugStore.Application.Handlers.Products;
using BugStore.Application.Requests.Products;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BugStore.Application.Tests;

public class ProductHandlerTests
{
    private static AppDbContext Ctx(string name = "product_handler_tests"){
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{name}_{Guid.CreateVersion7()}")
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task CreateProductAsync_Deve_Criar_Produto_Quando_Valido(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        var req = new CreateProductRequest("Titulo", "Desc", "slug", 10m);

        // Act
        var res = await handler.CreateProductAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal("Titulo", res.Data!.Title);
    }

    [Fact]
    public async Task CreateProductAsync_Deve_Retornar_Falha_Quando_DbUpdateException(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        var handler = new ProductHandler(mock.Object);
        var req = new CreateProductRequest("Titulo", "Desc", "slug", 10m);

        // Act
        var res = await handler.CreateProductAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao criar o produto. ErroCod: PH0001", res.Message);
    }

    [Fact]
    public async Task CreateProductAsync_Deve_Retornar_Falha_Quando_Cancelado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        var req = new CreateProductRequest("Titulo", "Desc", "slug", 10m);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var res = await handler.CreateProductAsync(req, cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task CreateProductAsync_Deve_Retornar_Falha_Quando_Erro_Desconhecido(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));
        var handler = new ProductHandler(mock.Object);
        var req = new CreateProductRequest("Titulo", "Desc", "slug", 10m);

        // Act
        var res = await handler.CreateProductAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro inesperado. ErroCod: PH0001-GEN", res.Message);
    }

    [Fact]
    public async Task GetProductByIdAsync_Deve_Retornar_Produto_Quando_Existir(){
        // Arrange
        await using var ctx = Ctx();
        var p = new Product("Titulo", "Desc", "slug", 10m);
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(ctx);
        var req = new GetProductByIdRequest(p.Id);

        // Act
        var res = await handler.GetProductByIdAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal(p.Id, res.Data!.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_Deve_Retornar_404_Quando_Nao_Encontrado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        var req = new GetProductByIdRequest(Guid.CreateVersion7());

        // Act
        var res = await handler.GetProductByIdAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Produto não encontrado.", res.Message);
    }

    [Fact]
    public async Task GetProductByIdAsync_Deve_Retornar_499_Quando_Cancelado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var res = await handler.GetProductByIdAsync(new GetProductByIdRequest(Guid.CreateVersion7()), cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task GetProductByIdAsync_Deve_Retornar_500_Quando_Erro(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        await ctx.DisposeAsync();

        // Act
        var res = await handler.GetProductByIdAsync(new GetProductByIdRequest(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao buscar o produto. ErroCod: PH0002", res.Message);
    }

    [Fact]
    public async Task GetAllProductsAsync_Deve_Retornar_Paginado(){
        // Arrange
        await using var ctx = Ctx();
        ctx.Products.AddRange(
            new Product("A", "D", "a", 1m),
            new Product("B", "D", "b", 2m),
            new Product("C", "D", "c", 3m)
        );
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(ctx);
        var req = new GetAllProductsRequest(pageNumber: 2, pageSize: 1);

        // Act
        var res = await handler.GetAllProductsAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal(3, res.TotalCount);
        Assert.Equal(1, res.PageSize);
        Assert.Equal(2, res.CurrentPage);
        Assert.Null(res.Message);
        Assert.Single(res.Data!);
        Assert.Equal("B", res.Data![0].Title);
    }

    [Fact]
    public async Task GetAllProductsAsync_Deve_Retornar_499_Quando_Cancelado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var res = await handler.GetAllProductsAsync(new GetAllProductsRequest(), cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Empty(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task GetAllProductsAsync_Deve_Retornar_500_Quando_Erro(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        await ctx.DisposeAsync();

        // Act
        var res = await handler.GetAllProductsAsync(new GetAllProductsRequest(), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Empty(res.Data);
        Assert.Equal("Erro ao listar os produtos. ErroCod: PH0003", res.Message);
    }

    [Fact]
    public async Task UpdateProductAsync_Deve_Atualizar_Quando_Existir(){
        // Arrange
        await using var ctx = Ctx();
        var p = new Product("Titulo", "Desc", "slug", 10m);
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(ctx);
        var req = new UpdateProductRequest( "Novo", null, null, 99m){
            Id = p.Id
        };

        // Act
        var res = await handler.UpdateProductAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal("Novo", res.Data!.Title);
        Assert.Equal(99m, res.Data!.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_Deve_Retornar_404_Quando_Nao_Encontrado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);
        var req = new UpdateProductRequest( null, null, null, null){
            Id = Guid.CreateVersion7()
        };

        // Act
        var res = await handler.UpdateProductAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Produto não encontrado.", res.Message);
    }

    [Fact]
    public async Task UpdateProductAsync_Deve_Retornar_Falha_Quando_DbUpdateException(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        var seed = new AppDbContext(opts);
        var p = new Product("T", "D", "s", 10m);
        seed.Products.Add(p);
        await seed.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(mock.Object);
        var req = new UpdateProductRequest("Novo", null, null, 20m){
            Id = p.Id
        };

        // Act
        var res = await handler.UpdateProductAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao atualizar o produto. ErroCod: PH0005", res.Message);
    }

    [Fact]
    public async Task UpdateProductAsync_Deve_Retornar_499_Quando_Cancelado(){
        // Arrange
        await using var ctx = Ctx();
        var p = new Product("T", "D", "s", 10m);
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(ctx);
        var req = new UpdateProductRequest("Novo", null, null, 20m){
            Id = p.Id
        };
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var res = await handler.UpdateProductAsync(req, cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task UpdateProductAsync_Deve_Retornar_500_Quando_Erro_Desconhecido(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));
        var seed = new AppDbContext(opts);
        var p = new Product("T", "D", "s", 10m);
        seed.Products.Add(p);
        await seed.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(mock.Object);
        var req = new UpdateProductRequest("Novo", null, null, 20m){
            Id = p.Id
        };

        // Act
        var res = await handler.UpdateProductAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro inesperado. ErroCod: PH0005-GEN", res.Message);
    }

    [Fact]
    public async Task DeleteProductAsync_Deve_Remover_Quando_Existir(){
        // Arrange
        await using var ctx = Ctx();
        var p = new Product("T", "D", "s", 10m);
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(ctx);
        var req = new DeleteProductRequest(p.Id);

        // Act
        var res = await handler.DeleteProductAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Produto removido com sucesso.", res.Message);
        Assert.Equal(0, await ctx.Products.CountAsync(CancellationToken.None));
    }

    [Fact]
    public async Task DeleteProductAsync_Deve_Retornar_404_Quando_Nao_Encontrado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new ProductHandler(ctx);

        // Act
        var res = await handler.DeleteProductAsync(new DeleteProductRequest(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Produto não encontrado.", res.Message);
    }

    [Fact]
    public async Task DeleteProductAsync_Deve_Retornar_Falha_Quando_DbUpdateException(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        var seed = new AppDbContext(opts);
        var p = new Product("T", "D", "s", 10m);
        seed.Products.Add(p);
        await seed.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(mock.Object);

        // Act
        var res = await handler.DeleteProductAsync(new DeleteProductRequest(p.Id), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao excluir o produto. ErroCod: PH0004", res.Message);
    }

    [Fact]
    public async Task DeleteProductAsync_Deve_Retornar_499_Quando_Cancelado(){
        // Arrange
        await using var ctx = Ctx();
        var p = new Product("T", "D", "s", 10m);
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(ctx);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var res = await handler.DeleteProductAsync(new DeleteProductRequest(p.Id), cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task DeleteProductAsync_Deve_Retornar_500_Quando_Erro_Desconhecido(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));
        var seed = new AppDbContext(opts);
        var p = new Product("T", "D", "s", 10m);
        seed.Products.Add(p);
        await seed.SaveChangesAsync(CancellationToken.None);
        var handler = new ProductHandler(mock.Object);

        // Act
        var res = await handler.DeleteProductAsync(new DeleteProductRequest(p.Id), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro inesperado. ErroCod: PH0004-GEN", res.Message);
    }
}
