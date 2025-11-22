using BugStore.Application.DTOs.Order.Requests;
using BugStore.Application.DTOs.OrderLine;
using BugStore.Application.Services;
using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BugStore.Application.Tests;

public class OrderServiceTests{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly OrderService _service;

    public OrderServiceTests(){
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _service = new OrderService(_orderRepositoryMock.Object, _customerRepositoryMock.Object,
            _productRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve criar um pedido com sucesso e retornar 201 quando o cliente existir e houver itens no pedido")]
    public async Task CreateOrderAsync_Should_Return_Success_When_Customer_And_OrderLines_Exist()
    {
        // Arrange
        var customer = new Customer("Maria", "maria@email.com", "11988887777",
            new DateTime(1985, 7, 25));

        var product1 = new Product("Teclado", "Teclado Mecânico", "teclado-mecanico", 250);
        var product2 = new Product("Mouse", "Mouse Gamer", "mouse-gamer", 150);

        var orderLinesDto = new List<OrderLineDto>
        {
            new() { ProductId = product1.Id, Quantity = 2, Price = product1.Price },
            new() { ProductId = product2.Id, Quantity = 1, Price = product2.Price }
        };

        var request = new CreateOrderRequest(customer.Id, orderLinesDto);

        var expectedLines = new List<OrderLine>
        {
            new(Guid.Empty, 2, product1),
            new(Guid.Empty, 1, product2)
        };

        var expectedOrder = new Order(customer, expectedLines);

        _customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);

        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product2);

        _orderRepositoryMock
            .Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _service.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data.Customer.Should().Be(customer);
        result.Data.Lines.Should().HaveCount(2);
        result.Data.Lines.Should().ContainSingle(l => l.ProductId == product1.Id && l.Quantity == 2);
        result.Data.Lines.Should().ContainSingle(l => l.ProductId == product2.Id && l.Quantity == 1);
        result.Message.Should().Be("Pedido criado com sucesso.");

        _orderRepositoryMock.Verify(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(p => p.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact(DisplayName = "Não deve criar o pedido e deve retornar 404 quando o cliente não existir. ErroCod: OS0001")]
    public async Task CreateOrderAsync_Should_Return_404_When_Customer_Not_Found()
    {
        // Arrange
        var request = new CreateOrderRequest(Guid.NewGuid(), []);

        _customerRepositoryMock
            .Setup(r => r.GetCustomerByIdAsync(request.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(404);
        result.Data.Should().BeNull();
        result.Message.Should().Contain("OS0001");
    }

    [Fact(DisplayName = "Não deve criar o pedido e deve retornar 400 quando o cliente existir e não houver itens. ErroCod: OS0002")]
    public async Task CreateOrderAsync_Should_Return_BadRequest_When_Customer_Exists_And_OrderLines_Is_Empty(){
        // Arrange
        var customer = new Customer("João", "joao@email.com", "11999999999",
            new DateTime(1990, 5, 10));
        var request = new CreateOrderRequest(customer.Id, []);

        _customerRepositoryMock
            .Setup(repo => repo.GetCustomerByIdAsync(customer.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(400);
        result.Data.Should().BeNull();
        result.Message.Should().Contain("Lista de produtos deve ter no mínimo 1 item.");

        _orderRepositoryMock.Verify(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _productRepositoryMock.Verify(p => p.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact(DisplayName = "Não deve criar o pedido e deve retornar 400 quando um produto da ordem não existir. ErroCod: OS0003")]
    public async Task CreateOrderAsync_Should_Return_400_When_Product_Not_Found(){
        // Arrange
        var customer = new Customer("João", "joao@email.com", "11999999999",
            new DateTime(1990, 5, 10));

        var request = new CreateOrderRequest(customer.Id, [
            new OrderLineDto { ProductId = Guid.NewGuid(), Quantity = 1 }
        ]);

        _customerRepositoryMock
            .Setup(r => r.GetCustomerByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null); // Produto não encontrado

        // Act
        var result = await _service.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(400);
        result.Data.Should().BeNull();
        result.Message.Should().Contain("OS0003");
    }

    [Fact(DisplayName = "Não deve criar o pedido e deve retornar 500 quando uma DbUpdateException(ErroCod: OS0004) for lançada ao salvar")]
    public async Task CreateOrderAsync_Should_Return_500_When_DbUpdateException_OS0004_Is_Thrown(){
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new Customer("João", "joao@email.com", "11999999999", new DateTime(1990, 5, 10));
        var product = new Product("Headset", "Headset Gamer", "headset-gamer", 500);
        var request = new CreateOrderRequest(customerId, [
            new OrderLineDto { ProductId = product.Id, Quantity = 1 }
        ]);

        _customerRepositoryMock
            .Setup(r => r.GetCustomerByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());

        // Act
        var result = await _service.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(500);
        result.Message.Should().Contain("OS0004");
        result.Data.Should().BeNull();

        _orderRepositoryMock.Verify(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(p => p.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact(DisplayName = "Não deve criar o pedido e deve retornar 400 quando uma OperationCanceledException(ErroCod: OS0005) for lançada ao salvar")]
    public async Task CreateOrderAsync_Should_Return_400_When_OperationCanceledException_OS0005_Is_Thrown()
    {
        // Arrange
        var customer = new Customer("João", "joao@email.com", "11999999999", new DateTime(1990, 5, 10));
        var product = new Product("Webcam", "Webcam FullHD", "webcam-fullhd", 350);
        var request = new CreateOrderRequest(customer.Id, [
            new OrderLineDto { ProductId = product.Id, Quantity = 1 }
        ]);

        _customerRepositoryMock
            .Setup(r => r.GetCustomerByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _service.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(400);
        result.Message.Should().Contain("OS0005");
        result.Data.Should().BeNull();

        _orderRepositoryMock.Verify(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Não deve criar o pedido e deve retornar 500 quando uma Exception (ErroCod: OS0006) for lançada ao salvar")]
    public async Task CreateOrderAsync_Should_Return_500_When_Exception_OS0006_Is_Thrown()
    {
        // Arrange
        var customer = new Customer("João", "joao@email.com", "11999999999", new DateTime(1990, 5, 10));
        var product = new Product("Monitor", "Monitor 4K", "monitor-4k", 2000);
        var request = new CreateOrderRequest(customer.Id, [
            new OrderLineDto { ProductId = product.Id, Quantity = 1 }
        ]);

        _customerRepositoryMock
            .Setup(r => r.GetCustomerByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetProductByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro genérico simulado"));

        // Act
        var result = await _service.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(500);
        result.Message.Should().Contain("OS0006");
        result.Data.Should().BeNull();

        _orderRepositoryMock.Verify(r =>
            r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar sucesso 200 quando o pedido for encontrado")]
    public async Task GetOrderByIdAsync_Should_Return_Success_When_Order_Exists(){
        // Arrange
        var customer = new Customer("João", "joao@email.com", "11999999999", new DateTime(1990, 5, 10));
        var product = new Product("SSD", "SSD NVMe", "ssd-nvme", 500);
        var line = new OrderLine(Guid.Empty, 1, product);
        var order = new Order(customer, [ line ]);
        var request = new GetOrderByIdRequest(order.Id);

        _orderRepositoryMock
            .Setup(r => r.GetOrderByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _service.GetOrderByIdAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(200);
        result.Data.Should().Be(order);
        result.Message.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Deve retornar 404 quando o pedido não for encontrado. ErroCod: OS0007")]
    public async Task GetOrderByIdAsync_Should_Return_404_When_Order_Not_Found(){
        // Arrange
        var request = new GetOrderByIdRequest(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(r => r.GetOrderByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _service.GetOrderByIdAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(404);
        result.Data.Should().BeNull();
        result.Message.Should().Contain("OS0007");
    }

    [Fact(DisplayName = "Deve retornar 400 quando a operação for cancelada. ErroCod: OS0008")]
    public async Task GetOrderByIdAsync_Should_Return_400_When_OperationCanceledException_Is_Thrown(){
        // Arrange
        var request = new GetOrderByIdRequest(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(r => r.GetOrderByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _service.GetOrderByIdAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(400);
        result.Data.Should().BeNull();
        result.Message.Should().Contain("OS0008");
    }

    [Fact(DisplayName = "Deve retornar 500 quando ocorrer erro inesperado. ErroCod: OS0009")]
    public async Task GetOrderByIdAsync_Should_Return_500_When_Exception_Is_Thrown(){
        // Arrange
        var request = new GetOrderByIdRequest(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(r => r.GetOrderByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _service.GetOrderByIdAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(500);
        result.Data.Should().BeNull();
        result.Message.Should().Contain("OS0009");
    }
}