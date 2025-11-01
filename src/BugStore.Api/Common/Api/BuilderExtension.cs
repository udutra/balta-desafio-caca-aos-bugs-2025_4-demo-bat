using BugStore.Application.Handlers.Customers;
using BugStore.Application.Handlers.Interfaces;
using BugStore.Application.Handlers.Orders;
using BugStore.Application.Handlers.Products;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Api.Common.Api;

public static class BuilderExtension{
    public static void AddConfiguration(this WebApplicationBuilder builder){
        ApiConfiguration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        ApiConfiguration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;

    }

    public static void AddDocumentation(this WebApplicationBuilder builder){
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x=> x.CustomSchemaIds(n => n.FullName));
    }

    public static void AddDataContexts(this WebApplicationBuilder builder){
        builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite(ApiConfiguration.ConnectionString));
    }

    public static void AddControllers(this WebApplicationBuilder builder){
        builder.Services.AddControllers();
    }

    public static void AddServices(this WebApplicationBuilder builder){
        builder.Services.AddScoped<IHandlerCustomer, CustomerHandler>();
        builder.Services.AddScoped<IHandlerOrder, OrderHandler>();
        builder.Services.AddScoped<IHandlerProduct, ProductHandler>();
    }
    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options => options.AddPolicy(
            ApiConfiguration.CorsPolicyName,
            policy => policy
                .WithOrigins([
                    ApiConfiguration.BackendUrl
                ])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
        ));
    }
}