using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Tests;

public class ProductRepositoryTests: IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task Create_Product_Async_Should_Add_Product_To_Database(){
        // Arrange
        var product = new Product("Mouse Gamer", "Mouse com RGB", "mouse-gamer",199.90m);

        // Act
        var result = await _repository.CreateProductAsync(product, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().NotBe(Guid.Empty);

        var saved = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id,
            cancellationToken: TestContext.Current.CancellationToken);
        saved.Should().NotBeNull();
        saved.Title.Should().Be("Mouse Gamer");
    }

    [Fact]
    public async Task Get_Product_By_Id_Async_Should_Return_Product_When_Exists()
    {
        // Arrange
        var product = new Product("Teclado Mecânico", "Switch azul", "teclado-mecanico", 299.99m);
        await _context.Products.AddAsync(product, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var found = await _repository.GetProductByIdAsync(product.Id, TestContext.Current.CancellationToken);

        // Assert
        found.Should().NotBeNull();
        found.Slug.Should().Be("teclado-mecanico");
    }

    [Fact]
    public async Task Get_Product_By_Id_Async_Should_Return_Null_When_Id_Is_Empty(){
        // Act
        var found = await _repository.GetProductByIdAsync(Guid.Empty, TestContext.Current.CancellationToken);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task Get_Product_By_Slug_Async_Should_Return_Product_When_Exists(){
        // Arrange
        var product = new Product("Monitor 4K", "Alta resolução", "monitor-4k",1299.00m);
        await _context.Products.AddAsync(product, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var found = await _repository.GetProductBySlugAsync("monitor-4k", TestContext.Current.CancellationToken);

        // Assert
        found.Should().NotBeNull();
        found.Title.Should().Be("Monitor 4K");
    }

    [Fact]
    public async Task Get_Product_By_Slug_Async_Should_Return_Null_When_Slug_Is_Null_Or_Empty(){
        // Act
        var found1 = await _repository.GetProductBySlugAsync("", TestContext.Current.CancellationToken);
        var found2 = await _repository.GetProductBySlugAsync(null!, TestContext.Current.CancellationToken);

        // Assert
        found1.Should().BeNull();
        found2.Should().BeNull();
    }

    [Fact]
    public async Task Get_All_Products_Should_Return_All_Products(){
        // Arrange
        await _context.Products.AddRangeAsync(
            new Product("Produto A", "Descrição A", "produto-a",  10m),
            new Product("Produto B", "Descrição B", "produto-b",  20m)
        );
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = _repository.GetAllProducts().ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.Title).Should().Contain(["Produto A", "Produto B"]);
    }

    [Fact]
    public async Task Update_Product_Async_Should_Modify_Existing_Product(){
        // Arrange
        var product = new Product("Cadeira", "Cadeira simples", "cadeira", 499m);
        await _context.Products.AddAsync(product, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        product.Title = "Cadeira Gamer";
        product.Description = "Com apoio de braço";
        product.Slug = "cadeira-gamer";
        product.Price = 799m;

        // Act
        var result = await _repository.UpdateProductAsync(product, TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(1);

        var updated = await _context.Products.FirstAsync(p => p.Id == product.Id,
            cancellationToken: TestContext.Current.CancellationToken);
        updated.Title.Should().Be("Cadeira Gamer");
        updated.Price.Should().Be(799m);
    }

    [Fact]
    public async Task Update_Product_Async_Should_Return_Zero_When_Not_Found(){
        // Arrange
        var product = new Product("Produto Inexistente", "slug", "Desc", 10m);

        // Act
        var result = await _repository.UpdateProductAsync(product, TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Delete_Product_Async_Should_Remove_Product_From_Database(){
        // Arrange
        var product = new Product("Headset", "headset", "Som 7.1", 399.90m);
        await _context.Products.AddAsync(product, TestContext.Current.CancellationToken);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _repository.DeleteProductAsync(product, TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(1);
        (await _context.Products.AnyAsync(p => p.Id == product.Id, cancellationToken:
            TestContext.Current.CancellationToken)).Should().BeFalse();
    }

    public void Dispose(){
        _context.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}