using System.Net;
using System.Net.Http.Json;
using BugStore.Application.DTOs;
using BugStore.Application.DTOs.Customer;
using BugStore.Application.DTOs.Customer.Requests;
using BugStore.Application.DTOs.Order.Requests;
using FluentAssertions;

namespace BugStore.Api.Tests.Controllers;

public class OrdersControllerTests(CustomWebApplicationFactory factory) : ApiTestBase(factory){

    [Fact(DisplayName = "POST /api/orders deve retornar 400 quando a requisição for BadRequest")]
    public async Task Create_Should_Return_400_When_BadRequest(){
        var customerRequest = new CreateCustomerRequest("João", "joao.orders@email.com", "11999990000", new DateTime(1990, 1, 1));
        var customerResponse = await Client.PostAsJsonAsync("/api/customers", customerRequest, TestContext.Current.CancellationToken);
        customerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCustomer = await Read<CustomerDto>(customerResponse);

        // Act
        var orderRequest = new CreateOrderRequest(createdCustomer.Id, []);
        var response = await Client.PostAsJsonAsync("/api/orders", orderRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await Read<ErrorDto>(response);
        body.StatusCode.Should().Be(400);
        body.Message.Should().Contain("Lista de produtos deve ter no mínimo 1 item.");
    }

    [Fact(DisplayName = "GET /api/orders/{id} deve retornar 404 quando não encontrado")]
    public async Task GetById_Should_Return_404_When_Not_Found()
    {
        // Arrange
        var anyId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/orders/{anyId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await Read<ErrorDto>(response);
        body.StatusCode.Should().Be(404);
        body.Message.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "GET /api/orders/{id} deve retornar 404 quando o id não for um GUID válido (rota não casada)")]
    public async Task GetById_Should_Return_404_When_Id_Is_Not_Guid()
    {
        // Act
        var response = await Client.GetAsync($"/api/orders/not-a-guid", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}