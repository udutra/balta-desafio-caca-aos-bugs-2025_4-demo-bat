using BugStore.Api;
using BugStore.Api.Common.Api;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.AddControllers();
builder.AddDataContexts();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();


var app = builder.Build();

if(app.Environment.IsDevelopment())
    app.ConfigureDevEnvironment();

app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapControllers();
app.Run();