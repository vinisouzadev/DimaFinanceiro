using Dima.API.Common;
using Dima.API.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder
    .AddConfiguration()
    .AddSecurity()
    .AddDataContexts()
    .AddCors()
    .AddSwagger()
    .AddDependencyInjection();

var app = builder.Build();

app.ConfigureDevEnvironment();

app.UseCors(ApiConfiguration.CorsPolicyName);

app.UseSecurity();

app.MapEndpoints();

app.Run();

public partial class Program() { }