using System.Net.Http.Json;
using BugStore.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BugStore.Api.Tests;

[Collection("ApiTests")]
public abstract class ApiTestBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected ApiTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async ValueTask InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    protected async Task<T> Read<T>(HttpResponseMessage message) =>
        await message.Content.ReadFromJsonAsync<T>()
        ?? throw new InvalidOperationException("Resposta inválida.");
}