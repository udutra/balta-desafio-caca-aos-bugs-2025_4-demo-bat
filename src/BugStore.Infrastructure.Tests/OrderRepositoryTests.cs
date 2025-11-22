using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Tests;

public class OrderRepositoryTests : IDisposable {
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests(){
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new OrderRepository(_context);
    }

    [Fact]
    public async Task Create_Order_Async_Should_Add_Order_To_Database(){
        // Arrange
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", new DateTime(1990, 5, 10));
        await _context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var order = new Order(customer, [
            new OrderLine(Guid.Empty, 3,
                new Product("P1", "D1", "p1", 5m))
        ]);

        // Act
        var result = await _repository.CreateOrderAsync(order, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Customer.Should().NotBeNull();

        var saved = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == order.Id, TestContext.Current.CancellationToken);

        saved.Should().NotBeNull();
        saved.Customer.Email.Should().Be("joao@email.com");
        (await _context.Orders.CountAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(1);
    }

    [Fact]
    public async Task Get_Order_By_Id_Async_Should_Return_Order_When_Exists(){
        // Arrange
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", new DateTime(1990, 5, 10));
        await _context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var order = new Order(customer, [
            new OrderLine(Guid.Empty, 3,
                new Product("P1", "D1", "p1", 5m))
        ]);
        await _context.Orders.AddAsync(order, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var found = await _repository.GetOrderByIdAsync(order.Id, TestContext.Current.CancellationToken);

        // Assert
        found.Should().NotBeNull();
        found.Customer.Email.Should().Be("joao@email.com");
    }

    public void Dispose(){
        _context.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}