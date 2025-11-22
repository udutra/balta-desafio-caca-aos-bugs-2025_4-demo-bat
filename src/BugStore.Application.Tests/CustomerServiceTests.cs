using BugStore.Application.DTOs.Customer.Requests;
using BugStore.Application.Services;
using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BugStore.Application.Tests;

public class CustomerServiceTests{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly CustomerService _service;

    public CustomerServiceTests(){
        _repositoryMock = new Mock<ICustomerRepository>();
        _service = new CustomerService(_repositoryMock.Object);
    }

    [Fact]
    public async Task Create_Customer_Async_Should_Return_Success_When_New_Customer()
    {
        // Arrange
        var request = new CreateCustomerRequest("João", "joao@email.com", "11999999999", new DateTime(1990, 1, 1));

        _repositoryMock
            .Setup(r => r.GetCustomerByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        _repositoryMock
            .Setup(r => r.GetCustomerByPhoneAsync(request.Phone, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        _repositoryMock
            .Setup(r => r.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        // Act
        var result = await _service.CreateCustomerAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("joao@email.com");

        _repositoryMock.Verify(r => r.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_Customer_Async_Should_Return_Conflict_When_Customer_Already_Exists()
    {
        // Arrange
        var existing = new Customer("Maria", "maria@email.com", "11911111111", new DateTime(1992, 1, 1));
        var request = new CreateCustomerRequest("Maria", "maria@email.com", "11911111111", new DateTime(1992, 1, 1));

        _repositoryMock.Setup(r => r.GetCustomerByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.CreateCustomerAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(409);
        result.Message.Should().Contain("Cliente já cadastrado");
    }

    [Fact]
    public async Task Get_Customer_By_IdAsync_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var request = new GetCustomerByIdRequest(Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetCustomerByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetCustomerByIdAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(404);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task Update_CustomerAsync_Should_Return_NoContent_When_Not_Modified(){
        // Arrange
        var customer = new Customer("José", "jose@email.com", "11988888888", new DateTime(1985, 1, 1));
        var request = new UpdateCustomerRequest("José", "jose@email.com", "11988888888", new DateTime(1985, 1, 1))
            {
                Id = customer.Id
            };

        _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customer.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _repositoryMock.Setup(r => r.UpdateCustomerAsync(customer,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _service.UpdateCustomerAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(204);
        result.Data.Should().BeNull();
    }


    [Fact]
    public async Task Delete_Customer_Async_Should_Return_Success_When_Removed()
    {
        // Arrange
        var customer = new Customer("Pedro", "pedro@email.com", "11999999999", new DateTime(1990, 5, 5));
        var request = new DeleteCustomerRequest(customer.Id);

        _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _repositoryMock.Setup(r => r.DeleteCustomerAsync(customer, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteCustomerAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(200);
        result.Data.Should().Be(customer);
    }

    [Fact]
    public async Task Delete_Customer_Async_Should_Return_NotFound_When_Customer_Not_Exists()
    {
        // Arrange
        var request = new DeleteCustomerRequest(Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetCustomerByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.DeleteCustomerAsync(request, CancellationToken.None);

        // Assert
        result.Code.Should().Be(404);
        result.Message.Should().Contain("Cliente não encontrado");
    }
}