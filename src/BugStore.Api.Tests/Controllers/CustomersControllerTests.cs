using System.Net;
using System.Net.Http.Json;
using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Customer;
using BugStore.Application.DTOs.Customer.Requests;
using BugStore.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BugStore.Api.Tests.Controllers;

public class CustomersControllerTests(CustomWebApplicationFactory factory) : ApiTestBase(factory){

    [Fact(DisplayName = "POST /api/customers deve retornar 201 ao criar um cliente")]
    public async Task Create_Should_Return_201(){
        var request = new CreateCustomerRequest("João", "joao@email.com", "11999999999",
            new DateTime(1990, 5, 10));

        var response = await Client.PostAsJsonAsync("/api/customers", request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await Read<CustomerDto>(response);
        body.Should().NotBeNull();
        body.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "POST /api/customers deve retornar 400 quando a requisição for inválida")]
    public async Task Create_Should_Return_400_When_Invalid_Request(){
        // Arrange
        var request = new CreateCustomerRequest("", "", "", DateTime.MinValue);

        // Act
        var response = await Client.PostAsJsonAsync("/api/customers", request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await Read<ValidationProblemDetails>(response);

        body.Errors.Should().ContainKey("Name");
        body.Errors.Should().ContainKey("Email");
        body.Errors.Should().ContainKey("Phone");
    }

    [Fact(DisplayName = "POST /api/customers deve retornar 409 quando já existir cliente com o mesmo e-mail")]
    public async Task Create_Should_Return_409_When_Email_Already_Exists(){
        var existing = new CreateCustomerRequest("Maria", "maria@email.com", "11988889999",
            new DateTime(1995, 1, 1));

        await Client.PostAsJsonAsync("/api/customers", existing, TestContext.Current.CancellationToken);

        var duplicate = new CreateCustomerRequest("Maria2", "maria@email.com", "11977776666",
            new DateTime(1998, 1, 1));

        var response = await Client.PostAsJsonAsync("/api/customers", duplicate,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var body = await Read<ErrorDto>(response);
        body.StatusCode.Should().Be(409);
        body.Message.Should().ContainEquivalentOf("Cliente já cadastrado. ErroCod: CS0001");
    }

    [Fact(DisplayName = "POST /api/customers deve retornar 500 se ocorrer um erro inesperado")]
    public async Task Create_Should_Return_500_When_Exception_Occurs(){
        // Arrange
        var factory = Factory.MockService<ICustomerService>(mock => {
            mock.Setup(s => s.CreateCustomerAsync(
                It.IsAny<CreateCustomerRequest>(),
                It.IsAny<CancellationToken>())
            ).ThrowsAsync(new Exception("Erro interno simulado"));
        });

        var client = factory.CreateClient();

        var request = new CreateCustomerRequest("Pedro", "pedro@email.com",
            "11999998888", new DateTime(1992, 3, 15));

        // Act
        var response = await client.PostAsJsonAsync("/api/customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var body = await Read<ErrorDto>(response);
        body.StatusCode.Should().Be(500);
        body.Message.Should().ContainEquivalentOf("simulado");
    }
}