using BugStore.Application.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BugStore.Application.Tests;

public class CustomerHandlerTests{
    private static AppDbContext Criar_Contexto(string nomeBd = "customer_handler_tests"){
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"{nomeBd}_{Guid.CreateVersion7()}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateCustomerAsync_Deve_Criar_Cliente_Quando_Valido()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);
        var req = new CreateCustomerRequest("Nome", "n@e.com", "111", new DateTime(2000,1,1));

        // Act
        var res = await handler.CreateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal("Cliente criado com sucesso.", res.Message);
        Assert.Equal(1, await ctx.Customers.CountAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateCustomerAsync_Deve_Retornar_409_Quando_Email_Ja_Existe()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        ctx.Customers.Add(new Customer("Nome", "dup@e.com", "111", new DateTime(2000,1,1)));
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(ctx);
        var req = new CreateCustomerRequest("Outro", "dup@e.com", "222", new DateTime(2001,1,1));

        // Act
        var res = await handler.CreateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Já existe um cliente com esse e-mail.", res.Message);
    }

    [Fact]
    public async Task CreateCustomerAsync_Deve_Retornar_Falha_Quando_DbUpdateException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(options) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        var handler = new CustomerHandler(mock.Object);
        var req = new CreateCustomerRequest("Nome", "e@e.com", "111", new DateTime(2000,1,1));

        // Act
        var res = await handler.CreateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao criar o cliente. ErroCod: CH0001", res.Message);
    }

    [Fact]
    public async Task CreateCustomerAsync_Deve_Retornar_Falha_Quando_Erro_Desconhecido()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(options) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));
        var handler = new CustomerHandler(mock.Object);
        var req = new CreateCustomerRequest("Nome", "e@e.com", "111", new DateTime(2000,1,1));

        // Act
        var res = await handler.CreateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro inesperado. ErroCod: CH0001-GEN", res.Message);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_Deve_Retornar_Cliente_Quando_Existir()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var c = new Customer("Nome", "id@e.com", "111", new DateTime(2000,1,1));
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(ctx);
        var req = new GetCustomerByIdRequest(c.Id);

        // Act
        var res = await handler.GetCustomerByIdAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal(c.Id, res.Data!.Id);
        Assert.Equal("Cliente localizado com sucesso.", res.Message);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_Deve_Retornar_404_Quando_Nao_Encontrado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);
        var req = new GetCustomerByIdRequest(Guid.CreateVersion7());

        // Act
        var res = await handler.GetCustomerByIdAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Cliente não encontrado.", res.Message);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_Deve_Retornar_499_Quando_Cancelado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var res = await handler.GetCustomerByIdAsync(new GetCustomerByIdRequest(Guid.CreateVersion7()), cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_Deve_Retornar_500_Quando_Erro()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);
        await ctx.DisposeAsync();

        // Act
        var res = await handler
            .GetCustomerByIdAsync(new GetCustomerByIdRequest(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Ocorreu um erro ao buscar o cliente. ErroCod: CH0002", res.Message);
    }

    [Fact]
    public async Task GetAllCustomersAsync_Deve_Retornar_Paginado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        ctx.Customers.AddRange(
            new Customer("A", "a@e.com", "111", new DateTime(2000,1,1)),
            new Customer("B", "b@e.com", "222", new DateTime(2000,1,1)),
            new Customer("C", "c@e.com", "333", new DateTime(2000,1,1))
        );
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(ctx);
        var req = new GetAllCustomersRequest(pageNumber: 2, pageSize: 1);

        // Act
        var res = await handler.GetAllCustomersAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal(3, res.TotalCount);
        Assert.Equal(1, res.PageSize);
        Assert.Equal(2, res.CurrentPage);
        Assert.Null(res.Message);
        Assert.Single(res.Data!);
        Assert.Equal("B", res.Data![0].Name);
    }

    [Fact]
    public async Task GetAllCustomersAsync_Deve_Retornar_499_Quando_Cancelado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var res = await handler.GetAllCustomersAsync(new GetAllCustomersRequest(), cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task GetAllCustomersAsync_Deve_Retornar_500_Quando_Erro()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);
        await ctx.DisposeAsync();

        // Act
        var res = await handler
            .GetAllCustomersAsync(new GetAllCustomersRequest(),CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Ocorreu um erro ao recuperar os clientes. ErroCod: CH0003", res.Message);
    }

    [Fact]
    public async Task UpdateCustomerAsync_Deve_Atualizar_Quando_Existir()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var c = new Customer("Nome", "x@e.com", "111", new DateTime(2000,1,1));
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(ctx);
        var req = new UpdateCustomerRequest("Novo Nome", null, null, null) { Id = c.Id };

        // Act
        var res = await handler.UpdateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Data);
        Assert.Equal("Novo Nome", res.Data!.Name);
        Assert.Equal("Cliente atualizado com sucesso.", res.Message);
    }

    [Fact]
    public async Task UpdateCustomerAsync_Deve_Retornar_404_Quando_Nao_Encontrado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);
        var req = new UpdateCustomerRequest(null, null, null, null) { Id = Guid.CreateVersion7() };

        // Act
        var res = await handler.UpdateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Cliente não encontrado.", res.Message);
    }

    [Fact]
    public async Task UpdateCustomerAsync_Deve_Retornar_499_Quando_Cancelado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var c = new Customer("Nome", "y@e.com", "111", new DateTime(2000,1,1));
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(ctx);
        var req = new UpdateCustomerRequest("Novo", null, null, null) { Id = c.Id };
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var res = await handler.UpdateCustomerAsync(req, cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task UpdateCustomerAsync_Deve_Retornar_Falha_Quando_DbUpdateException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(options) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        var seed = new AppDbContext(options);
        seed.Customers.Add(new Customer("Nome", "z@e.com", "111", new DateTime(2000,1,1)));
        await seed.SaveChangesAsync(CancellationToken.None);
        var cliente = await seed.Customers.AsNoTracking().FirstAsync(CancellationToken.None);
        var handler = new CustomerHandler(mock.Object);
        var req = new UpdateCustomerRequest("Novo", null, null, null) { Id = cliente.Id };

        // Act
        var res = await handler.UpdateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao atualizar o cliente. ErroCod: CH0004", res.Message);
    }

    [Fact]
    public async Task UpdateCustomerAsync_Deve_Retornar_500_Quando_Erro_Desconhecido()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(options) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));
        var seed = new AppDbContext(options);
        seed.Customers.Add(new Customer("Nome", "zz@e.com", "111", new DateTime(2000,1,1)));
        await seed.SaveChangesAsync(CancellationToken.None);
        var cliente = await seed.Customers.AsNoTracking().FirstAsync(CancellationToken.None);
        var handler = new CustomerHandler(mock.Object);
        var req = new UpdateCustomerRequest("Novo", null, null, null) { Id = cliente.Id };

        // Act
        var res = await handler.UpdateCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro inesperado. ErroCod: CH0004-GEN", res.Message);
    }

    [Fact]
    public async Task DeleteCustomerAsync_Deve_Remover_Quando_Existir()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var c = new Customer("Nome", "d@e.com", "111", new DateTime(2000,1,1));
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(ctx);
        var req = new DeleteCustomerRequest(c.Id);

        // Act
        var res = await handler.DeleteCustomerAsync(req, CancellationToken.None);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Cliente removido com sucesso.", res.Message);
        Assert.Equal(0, await ctx.Customers.CountAsync(CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCustomerAsync_Deve_Retornar_404_Quando_Nao_Encontrado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var handler = new CustomerHandler(ctx);

        // Act
        var res = await handler
            .DeleteCustomerAsync(new DeleteCustomerRequest(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Cliente não encontrado.", res.Message);
    }

    [Fact]
    public async Task DeleteCustomerAsync_Deve_Retornar_499_Quando_Cancelado()
    {
        // Arrange
        await using var ctx = Criar_Contexto();
        var c = new Customer("Nome", "del@e.com", "111", new DateTime(2000,1,1));
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(ctx);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var res = await handler.DeleteCustomerAsync(new DeleteCustomerRequest(c.Id), cts.Token);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Operação cancelada.", res.Message);
    }

    [Fact]
    public async Task DeleteCustomerAsync_Deve_Retornar_500_Quando_Erro()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(options) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        var seed = new AppDbContext(options);
        var c = new Customer("Nome", "d2@e.com", "111", new DateTime(2000,1,1));
        seed.Customers.Add(c);
        await seed.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(mock.Object);

        // Act
        var res = await handler.DeleteCustomerAsync(new DeleteCustomerRequest(c.Id), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro ao remover o cliente. ErroCod: CH0005", res.Message);
    }

    [Fact]
    public async Task DeleteCustomerAsync_Deve_Retornar_500_Quando_Erro_Desconhecido()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ex_{Guid.CreateVersion7()}").Options;
        var mock = new Mock<AppDbContext>(options) { CallBase = true };
        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));
        var seed = new AppDbContext(options);
        var c = new Customer("Nome", "d3@e.com", "111", new DateTime(2000,1,1));
        seed.Customers.Add(c);
        await seed.SaveChangesAsync(CancellationToken.None);
        var handler = new CustomerHandler(mock.Object);

        // Act
        var res = await handler.DeleteCustomerAsync(new DeleteCustomerRequest(c.Id), CancellationToken.None);

        // Assert
        Assert.False(res.IsSuccess);
        Assert.Null(res.Data);
        Assert.Equal("Erro inesperado. ErroCod: CH0005-GEN", res.Message);
    }
}
