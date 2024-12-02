using Bogus;
using Dima.API.Data;
using Dima.Core.Handlers;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using Dima.Core.Responses.Stripe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using NSubstitute;
using Respawn;
using Testcontainers.PostgreSql;

namespace Dima.API.IntegrationTests.ApplicationFactory
{

    public class DimaFinanceiroApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder().Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {   
                var dbContextOptionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                services.Remove(dbContextOptionsDescriptor!);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(_postgresContainer.GetConnectionString());
                });

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.Migrate();

                var mockStripeHandler = Substitute.For<IStripeHandler>();
                List<StripeTransactionResponse> listStripeTransactionResponse =
                [
                    new()
                    {
                        Id = "123456789",
                        Paid = true,
                        Refunded = false
                    }
                ];
                Response<List<StripeTransactionResponse>> stripeTransactionResponse = new(listStripeTransactionResponse, 200, string.Empty);
                mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Returns(stripeTransactionResponse);

                services.Replace(ServiceDescriptor.Transient<IStripeHandler>(x => mockStripeHandler));
            });

        }

        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _postgresContainer.StopAsync();
        }

        public async Task DatabaseClearAsync()
        {
            using var connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
            connection.Open();
            var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions()
            {
                DbAdapter = DbAdapter.Postgres
            });

            await respawner.ResetAsync(connection);
        }
    }
}
