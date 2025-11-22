using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Tests;

public class CustomerRepositoryTests : IDisposable{

    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests(){
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task Create_Customer_Async_Should_Add_Customer_To_Database(){
        // Arrange
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", new DateTime(1990, 5, 10));

        // Act
        var result = await _repository.CreateCustomerAsync(customer, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        var saved = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email, cancellationToken: TestContext.Current.CancellationToken);
        saved.Should().NotBeNull();
        saved.Name.Should().Be("João Silva");
    }

    [Fact]
    public async Task Get_Customer_By_Id_Async_Should_Return_Customer_When_Exists(){
        // Arrange
        var customer = new Customer("Maria", "maria@test.com", "11988887777", new DateTime(1985, 3, 15));
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var found = await _repository.GetCustomerByIdAsync(customer.Id, TestContext.Current.CancellationToken);

        // Assert
        found.Should().NotBeNull();
        found.Email.Should().Be("maria@test.com");
    }

    [Fact]
    public async Task Get_Customer_By_Email_Async_Should_Return_Null_When_Not_Found(){
        // Act
        var result = await _repository.GetCustomerByEmailAsync("naoexiste@email.com", TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_Customer_Async_Should_Update_Only_Modified_Fields(){
        // Arrange
        var customer = new Customer("Pedro", "pedro@email.com", "11977776666", new DateTime(1980, 1, 1));
        await _repository.CreateCustomerAsync(customer, TestContext.Current.CancellationToken);

        // Act
        customer.Name = "Pedro Novo";
        var result = await _repository.UpdateCustomerAsync(customer, TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(1);
        var updated = await _context.Customers.FindAsync([customer.Id], TestContext.Current.CancellationToken);
        updated.Should().NotBeNull();
        updated.Name.Should().Be("Pedro Novo");
    }

    [Fact]
    public async Task Delete_Customer_Async_Should_Remove_Customer(){
        // Arrange
        var customer = new Customer("Ana", "ana@test.com", "1188888888", new DateTime(1995, 9, 1));
        await _repository.CreateCustomerAsync(customer, TestContext.Current.CancellationToken);

        // Act
        var result = await _repository.DeleteCustomerAsync(customer, TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(1);
        var deleted = await _context.Customers.FindAsync([customer.Id], TestContext.Current.CancellationToken);
        deleted.Should().BeNull();
    }

    public void Dispose(){
        _context.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}