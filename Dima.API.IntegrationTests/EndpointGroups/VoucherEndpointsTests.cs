using Bogus;
using Dima.API.Data;
using Dima.Core.Requests.Account;
using System.Text;
using System.Text.Json;
using Dima.Core.Models.Orders;
using FluentAssertions;
using System.Net.Http.Json;
using Dima.Core.Responses;
using Dima.API.IntegrationTests.ApplicationFactory;

namespace Dima.API.IntegrationTests.EndpointGroups
{
    [Trait("Endpoint", "Voucher")]
    public class VoucherEndpointsTests : IClassFixture<DimaFinanceiroApplicationFactory>
    {
        private readonly DimaFinanceiroApplicationFactory _webApplicationFactory;

        private readonly Faker _faker = new("pt_BR");

        public VoucherEndpointsTests(DimaFinanceiroApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task GET_GetByCode_WithValidCode_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            Voucher voucher = CreateVoucher();

            var response = await client.GetAsync($"v1/vouchers/{voucher.VourcherCode}");
            var responseData = await response.Content.ReadFromJsonAsync<Response<Voucher>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseData!.Data!.Title.Should().Be(voucher.Title);
        }

        [Fact]
        public async Task GET_GetByCode_WithInvalidCode_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            Voucher voucher = CreateVoucher();

            var response = await client.GetAsync($"v1/vouchers/{_faker.Random.Utf16String(minLength: 1, maxLength: 2)}");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_GetByCode_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            Voucher voucher = CreateVoucher();
            var response = await client.GetAsync($"v1/vouchers/{voucher.VourcherCode}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        }

        private Voucher CreateVoucher()
        {
            using var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Voucher voucher = new Voucher()
            {
                VourcherCode = _faker.Random.Hexadecimal(5),
                Amount = _faker.Random.Decimal(1, 10),
                Title = _faker.Person.FirstName,
                Description = _faker.Person.LastName,
                IsActive = true
            };
            context.Vouchers.Add(voucher);
            context.SaveChanges();
            return voucher;
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
        }
    }
}
