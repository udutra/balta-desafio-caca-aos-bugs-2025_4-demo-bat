using BugStore.Api.Common.Api;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.AddDataContexts();
builder.AddControllers();
builder.AddRepositories();
builder.AddServices();
builder.AddAutoMapperProfiles();
builder.AddDocumentation();
builder.AddCrossOrigin();

var app = builder.Build();

if(app.Environment.IsDevelopment())
    app.ConfigureDevEnvironment();

app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapControllers();
app.Run();