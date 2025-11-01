using BugStore.Api.Common.Api;
using BugStore.Api.Endpoints.Customers;
using BugStore.Api.Endpoints.Orders;
using BugStore.Api.Endpoints.Products;

namespace BugStore.Api.Endpoints;

public static class Endpoint{
    public static void MapEndpoints(this WebApplication app){
        var endpoints = app.MapGroup("");

        endpoints.MapGroup("")
            .WithTags("Health Check")
            .MapGet("/", () => new {message = "OK"});

        endpoints.MapGroup("/v1/customers")
            .WithTags("Customers")
            .MapEndpoint<CreateCustomerEndPoint>()
            .MapEndpoint<GetCustomerByIdEndPoint>()
            .MapEndpoint<GetAllCustomerEndPoint>()
            .MapEndpoint<UpdateCustomerEndPoint>()
            .MapEndpoint<DeleteCustomerEndPoint>();

        endpoints.MapGroup("/v1/orders")
            .WithTags("Orders")
            .MapEndpoint<CreateOrderEndPoint>()
            .MapEndpoint<GetOrderByIdEndPoint>();

        endpoints.MapGroup("/v1/products")
            .WithTags("Products")
            .MapEndpoint<CreateProductEndPoint>()
            .MapEndpoint<GetAllProductsEndPoint>()
            .MapEndpoint<GetProductByIdEndPoint>()
            .MapEndpoint<UpdateProductEndPoint>()
            .MapEndpoint<DeleteProductEndPoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint :
        IEndpoint{
        TEndpoint.Map(app);
        return app;
    }
}