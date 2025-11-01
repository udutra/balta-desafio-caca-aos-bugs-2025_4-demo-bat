namespace BugStore.Api.Common.Api;

public static class AppExtension{
    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.UseSwagger(options => options.OpenApiVersion =
            Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
        app.UseSwaggerUI();
        app.MapSwagger().RequireAuthorization();
    }
}