using Dima.API.Common;
using Dima.API.Endpoints;

internal class Program
{
    private static void Main(string[] args)
    {
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
    }
}