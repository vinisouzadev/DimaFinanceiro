using Bogus;
using Dima.API.Data;
using Dima.API.IntegrationTests.ApplicationFactory;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Account;
using Dima.Core.Responses;
using FluentAssertions;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Dima.API.IntegrationTests.EndpointGroups
{
    [Trait("Endpoint", "Product")]
    public class ProductEndpointsTests : IClassFixture<DimaFinanceiroApplicationFactory>
    {
        private readonly DimaFinanceiroApplicationFactory _webApplicationFactory;

        private readonly Faker _faker = new("pt_BR");

        public ProductEndpointsTests(DimaFinanceiroApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;

        }

        [Fact]
        public async Task GET_GetBySlug_WithValidSlug_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            using var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Products.Add(new Product()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Model(),
                IsActive = true,
                Price = _faker.Random.Decimal(1, 1000),
                Slug = "Slug1"
            });
            context.SaveChanges();

            var response = await client.GetAsync($"v1/products/Slug1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseData = await response.Content.ReadFromJsonAsync<Response<Product?>>();
            responseData!.Data.Should().NotBeNull();
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetBySlug_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            var response = await client.GetAsync("v1/products/Slug1");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GET_GetAll_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            using var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Products.Add(new Product()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer(),
                IsActive = true,
                Price = _faker.Random.Decimal(1, 1000),
                Slug = "Slug1"
            });
            context.Products.Add(new Product()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer(),
                IsActive = true,
                Price = _faker.Random.Decimal(1, 1000),
                Slug = "Slug2"
            });
            context.SaveChanges();

            var response = await client.GetAsync("v1/products");
            var responseData = await response.Content.ReadFromJsonAsync<PagedResponse<List<Product>>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseData!.Data.Should().NotBeNull();
            responseData.TotalCount.Should().Be(2);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetAll_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            var response = await client.GetAsync("v1/products");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task ClientAuthentication(HttpClient client)
        {
            RegisterRequest registerRequest = new()
            {
                Email = _faker.Person.Email,
                Password = "Teste1."
            };
            string registerBody = JsonSerializer.Serialize(registerRequest);
            StringContent registerStringContent = new(registerBody, Encoding.UTF8, "application/json");
            var registerResponse = await client.PostAsync("v1/identity/register", registerStringContent);

            LoginRequest loginRequest = new()
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password,
            };
            string loginBody = JsonSerializer.Serialize(loginRequest);
            StringContent loginStringContent = new(loginBody, Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("v1/identity/login?useCookies=true", loginStringContent);
            var cookies = loginResponse.Headers.GetValues("Set-Cookie");
        }
    }
}
