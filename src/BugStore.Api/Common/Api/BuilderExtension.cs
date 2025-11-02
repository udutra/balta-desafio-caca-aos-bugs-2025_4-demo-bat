using BugStore.Application.Interfaces;
using BugStore.Application.Mappings;
using BugStore.Application.Services;
using BugStore.Domain.Interfaces;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Api.Common.Api;

public static class BuilderExtension{
    public static void AddConfiguration(this WebApplicationBuilder builder){
        ApiConfiguration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        ApiConfiguration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;

    }

    public static void AddDataContexts(this WebApplicationBuilder builder){
        builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite(ApiConfiguration.ConnectionString));
    }

    public static void AddControllers(this WebApplicationBuilder builder){
        builder.Services.AddControllers();
    }

    public static void AddRepositories(this WebApplicationBuilder builder){
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
    }

    public static void AddServices(this WebApplicationBuilder builder){
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IProductService, ProductService>();
    }

    public static void AddAutoMapperProfiles(this WebApplicationBuilder builder){
        builder.Services.AddAutoMapper(cfg => {
            cfg.AddProfile<CustomerMappingProfile>();
            cfg.AddProfile<OrderMappingProfile>();
            cfg.AddProfile<ProductMappingProfile>();
        });
    }

    public static void AddDocumentation(this WebApplicationBuilder builder){
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x=> x.CustomSchemaIds(n => n.FullName));
    }

    public static void AddCrossOrigin(this WebApplicationBuilder builder){
        builder.Services.AddCors(options => options.AddPolicy(ApiConfiguration.CorsPolicyName,
            policy => policy.WithOrigins([ApiConfiguration.BackendUrl]).AllowAnyMethod()
                .AllowAnyHeader().AllowCredentials()));
    }
}