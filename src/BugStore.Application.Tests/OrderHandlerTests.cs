using BugStore.Application.Handlers.Orders;
using BugStore.Application.Requests.Orders;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BugStore.Application.Tests;

public class OrderHandlerTests
{
    private static AppDbContext Ctx(string name = "order_handler_tests"){
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{name}_{Guid.CreateVersion7()}")
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task CreateOrderAsync_Deve_Criar_Pedido_Quando_Valido(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new OrderHandler(ctx);
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var produto = new Product("P", "D", "p", 10m);
        var linhas = new List<OrderLine>{ new(Guid.Empty, 2, produto) };
        var req = new CreateOrderRequest(cliente.Id, cliente, linhas);

        // Act
        var res = await handler.CreateOrderAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal(cliente.Id, res.Data!.CustomerId);
        Assert.Single(res.Data!.Lines);
        Assert.Equal(2, res.Data!.Lines.First().Quantity);
        Assert.Equal("Pedido criado com sucesso.", res.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_Deve_Retornar_Falha_Quando_DbUpdateException(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        var handler = new OrderHandler(mock.Object);
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var produto = new Product("P", "D", "p", 10m);
        var linhas = new List<OrderLine>{ new(Guid.Empty, 1, produto) };
        var req = new CreateOrderRequest(cliente.Id, cliente, linhas);

        // Act
        var res = await handler.CreateOrderAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao criar o pedido. ErroCod: OH0001", res.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_Deve_Retornar_499_Quando_Cancelado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new OrderHandler(ctx);
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var produto = new Product("P", "D", "p", 10m);
        var linhas = new List<OrderLine>{ new(Guid.Empty, 1, produto) };
        var req = new CreateOrderRequest(cliente.Id, cliente, linhas);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var res = await handler.CreateOrderAsync(req, cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_Deve_Retornar_Falha_Quando_Erro_Desconhecido(){
        // Arrange
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(opts) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));
        var handler = new OrderHandler(mock.Object);
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var produto = new Product("P", "D", "p", 10m);
        var linhas = new List<OrderLine>{ new(Guid.Empty, 1, produto) };
        var req = new CreateOrderRequest(cliente.Id, cliente, linhas);

        // Act
        var res = await handler.CreateOrderAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro inesperado. ErroCod: OH0001-GEN", res.Message);
    }

    [Fact]
    public async Task GetOrderByIdAsync_Deve_Retornar_Pedido_Quando_Existir(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new OrderHandler(ctx);
        var cliente = new Customer("Cliente", "c@c.com", "111", new DateTime(2000,1,1));
        var produto = new Product("P", "D", "p", 10m);
        var linhas = new List<OrderLine>{ new(Guid.Empty, 2, produto) };
        var created = await handler.CreateOrderAsync(new CreateOrderRequest(cliente.Id, cliente, linhas), CancellationToken.None);
        var req = new GetOrderByIdRequest(created.Data!.Id);

        // Act
        var res = await handler.GetOrderByIdAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal(created.Data!.Id, res.Data!.Id);
        Assert.Equal("Pedido encontrado com sucesso.", res.Message);
    }

    [Fact]
    public async Task GetOrderByIdAsync_Deve_Retornar_404_Quando_Nao_Encontrado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new OrderHandler(ctx);
        var req = new GetOrderByIdRequest(Guid.CreateVersion7());

        // Act
        var res = await handler.GetOrderByIdAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Pedido não encontrado.", res.Message);
    }

    [Fact]
    public async Task GetOrderByIdAsync_Deve_Retornar_499_Quando_Cancelado(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new OrderHandler(ctx);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var res = await handler.GetOrderByIdAsync(new GetOrderByIdRequest(Guid.CreateVersion7()), cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task GetOrderByIdAsync_Deve_Retornar_500_Quando_Erro(){
        // Arrange
        await using var ctx = Ctx();
        var handler = new OrderHandler(ctx);
        await ctx.DisposeAsync();

        // Act
        var res = await handler
            .GetOrderByIdAsync(new GetOrderByIdRequest(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Ocorreu um erro ao buscar o pedido. ErroCod: OH0002", res.Message);
    }
}

