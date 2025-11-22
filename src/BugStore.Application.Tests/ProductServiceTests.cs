using BugStore.Application.DTOs.Product.Requests;
using BugStore.Application.Services;
using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;

namespace BugStore.Application.Tests;

public class ProductServiceTests{

    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Deve criar um produto com sucesso")]
    public async Task CreateProductAsync_Should_Return_Success_When_Product_Is_Created()
    {
        var request = new CreateProductRequest("Notebook", "Gamer", "notebook-gamer", 5000);

        _repositoryMock.Setup(r => r.GetProductBySlugAsync(request.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _repositoryMock.Setup(r => r.CreateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product(request.Title, request.Description, request.Slug, request.Price));

        var result = await _service.CreateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(200);
        result.Data.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve criar produto quando o slug já existir")]
    public async Task CreateProductAsync_Should_Return_409_When_Slug_Exists()
    {
        var request = new CreateProductRequest("Notebook", "Gamer", "notebook-gamer", 5000);

        _repositoryMock.Setup(r => r.GetProductBySlugAsync(request.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product("a","b","notebook-gamer",10));

        var result = await _service.CreateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(409);
        result.Message.Should().Contain("PS0001");
    }

    [Fact(DisplayName = "Deve retornar 500 quando ocorrer DbUpdateException ao criar produto")]
    public async Task CreateProductAsync_Should_Return_500_When_DbUpdateException_Is_Thrown()
    {
        var request = new CreateProductRequest("A", "B", "slug", 10);

        _repositoryMock.Setup(r => r.GetProductBySlugAsync(request.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _repositoryMock.Setup(r => r.CreateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());

        var result = await _service.CreateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(500);
        result.Message.Should().Contain("PS0002");
    }

    [Fact(DisplayName = "Deve retornar 400 quando operação for cancelada ao criar produto")]
    public async Task CreateProductAsync_Should_Return_400_When_OperationCanceledException_Is_Thrown()
    {
        var request = new CreateProductRequest("A", "B", "slug", 10);

        _repositoryMock.Setup(r => r.GetProductBySlugAsync(request.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _repositoryMock.Setup(r => r.CreateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _service.CreateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(400);
        result.Message.Should().Contain("PS0003");
    }

    [Fact(DisplayName = "Deve retornar 500 para erro inesperado ao criar produto")]
    public async Task CreateProductAsync_Should_Return_500_When_Exception_Is_Thrown()
    {
        var request = new CreateProductRequest("A", "B", "slug", 10);

        _repositoryMock.Setup(r => r.GetProductBySlugAsync(request.Slug, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var result = await _service.CreateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(500);
        result.Message.Should().Contain("PS0004");
    }

    [Fact(DisplayName = "Deve retornar o produto quando encontrado")]
    public async Task GetProductByIdAsync_Should_Return_Product_When_Found()
    {
        var id = Guid.NewGuid();
        var request = new GetProductByIdRequest(id);

        var product = new Product("A", "B", "slug", 10);

        _repositoryMock.Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _service.GetProductByIdAsync(request, CancellationToken.None);

        result.Code.Should().Be(200);
        result.Data.Should().Be(product);
    }

    [Fact(DisplayName = "Deve retornar 400 quando o Id for inválido")]
    public async Task GetProductByIdAsync_Should_Return_400_When_Id_Is_Invalid()
    {
        var request = new GetProductByIdRequest(Guid.Empty);

        var result = await _service.GetProductByIdAsync(request, CancellationToken.None);

        result.Code.Should().Be(400);
        result.Message.Should().Contain("PS0005");
    }

    [Fact(DisplayName = "Deve retornar 400 quando operação for cancelada ao buscar produto")]
    public async Task GetProductByIdAsync_Should_Return_400_When_OperationCanceledException_Is_Thrown()
    {
        var id = Guid.NewGuid();
        var request = new GetProductByIdRequest(id);

        _repositoryMock.Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _service.GetProductByIdAsync(request, CancellationToken.None);

        result.Code.Should().Be(400);
        result.Message.Should().Contain("PS0007");
    }

    [Fact(DisplayName = "Deve retornar 500 quando ocorrer erro inesperado ao buscar produto")]
    public async Task GetProductByIdAsync_Should_Return_500_When_Exception_Is_Thrown()
    {
        var id = Guid.NewGuid();
        var request = new GetProductByIdRequest(id);

        _repositoryMock.Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var result = await _service.GetProductByIdAsync(request, CancellationToken.None);

        result.Code.Should().Be(500);
        result.Message.Should().Contain("PS0008");
    }

    [Fact(DisplayName = "Deve criar produto com sucesso e retornar 200")]
    public async Task CreateProductAsync_Should_Create_When_Slug_Not_Exists()
    {
        // Arrange
        var request = new CreateProductRequest("Produto A", "Desc", "produto-a", 10m);

        _repositoryMock
            .Setup(r => r.GetProductBySlugAsync(request.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _repositoryMock
            .Setup(r => r.CreateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        // Act
        var result = await _service.CreateProductAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(200);
        result.Data.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve criar produto e deve retornar 500 quando ocorrer DbUpdateException")]
    public async Task CreateProductAsync_Should_Return_500_When_DbUpdateException()
    {
        var request = new CreateProductRequest("A","B","c",10);

        _repositoryMock
            .Setup(r => r.GetProductBySlugAsync(request.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _repositoryMock
            .Setup(r => r.CreateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());

        var result = await _service.CreateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(500);
        result.Message.Should().Contain("PS0002");
    }

    [Fact(DisplayName = "Deve retornar produto com sucesso")]
    public async Task GetProductByIdAsync_Should_Return_Product()
    {
        var id = Guid.NewGuid();
        var product = new Product("A","B","a-b",10);

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _service.GetProductByIdAsync(new GetProductByIdRequest(id), CancellationToken.None);

        result.Code.Should().Be(200);
        result.Data.Should().Be(product);
    }

    [Fact(DisplayName = "Deve retornar 400 quando Id for inválido")]
    public async Task GetProductByIdAsync_Should_Return_400_When_Id_Invalid()
    {
        var result = await _service.GetProductByIdAsync(new GetProductByIdRequest(Guid.Empty), CancellationToken.None);

        result.Code.Should().Be(400);
        result.Message.Should().Contain("PS0005");
    }

    [Fact(DisplayName = "Deve retornar 404 quando produto não encontrado")]
    public async Task GetProductByIdAsync_Should_Return_404_When_Not_Found()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _service.GetProductByIdAsync(new GetProductByIdRequest(id), CancellationToken.None);

        result.Code.Should().Be(404);
        result.Message.Should().Contain("PS0006");
    }

    [Fact(DisplayName = "Deve retornar lista paginada com sucesso")]
    public async Task GetAllProductsAsync_Should_Return_Paged_List(){
        var products = new List<Product>{
            new("A","B","a",10),
            new("C","D","c",20),
        }.BuildMock();

        _repositoryMock.Setup(r => r.GetAllProducts()).Returns(products);

        var request = new GetAllProductsRequest(1, 10);
        var result = await _service.GetAllProductsAsync(request, CancellationToken.None);

        result.Code.Should().Be(200);
        result.Data.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Deve retornar 400 quando parâmetros de paginação forem inválidos")]
    public async Task GetAllProductsAsync_Should_Return_400_When_Pagination_Invalid()
    {
        var request = new GetAllProductsRequest(0, 0);

        var result = await _service.GetAllProductsAsync(request, CancellationToken.None);

        result.Code.Should().Be(400);
        result.Message.Should().Contain("PS0009");
    }

    [Fact(DisplayName = "Deve atualizar produto e retornar 200")]
    public async Task UpdateProductAsync_Should_Update()
    {
        var product = new Product("A","B","slug",10);
        var request = new UpdateProductRequest("Novo", "Desc", "slug", 20){
            Id = product.Id
        };

        _repositoryMock.Setup(r => r.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repositoryMock.Setup(r => r.UpdateProductAsync(product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.UpdateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(200);
        result.Data.Should().Be(product);
    }

    [Fact(DisplayName = "Deve retornar 204 quando nada for modificado")]
    public async Task UpdateProductAsync_Should_Return_204_When_Not_Modified()
    {
        var product = new Product("A","B","slug",10);
        var request = new UpdateProductRequest("Novo", "Desc", "slug", 20){
            Id = product.Id
        };

        _repositoryMock.Setup(r => r.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repositoryMock.Setup(r => r.UpdateProductAsync(product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _service.UpdateProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(204);
        result.Message.Should().Contain("PS0014");
    }

    [Fact(DisplayName = "Deve deletar produto e retornar 200")]
    public async Task DeleteProductAsync_Should_Delete()
    {
        var product = new Product("A","B","slug",10);
        var request = new DeleteProductRequest(product.Id);

        _repositoryMock.Setup(r => r.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repositoryMock.Setup(r => r.DeleteProductAsync(product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.DeleteProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(200);
    }

    [Fact(DisplayName = "Deve retornar 404 quando produto não encontrado ao remover")]
    public async Task DeleteProductAsync_Should_Return_404_When_Not_Found()
    {
        var id = Guid.NewGuid();
        var request = new DeleteProductRequest(id);

        _repositoryMock.Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _service.DeleteProductAsync(request, CancellationToken.None);

        result.Code.Should().Be(404);
        result.Message.Should().Contain("CS0020");
    }

}