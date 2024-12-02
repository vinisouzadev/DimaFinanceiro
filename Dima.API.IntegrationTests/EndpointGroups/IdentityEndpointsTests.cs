using Bogus;
using Dima.API.IntegrationTests.ApplicationFactory;
using Dima.API.Models;
using Dima.Core.Models.Account;
using Dima.Core.Requests.Account;
using FluentAssertions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Dima.API.IntegrationTests.EndpointGroups
{
    [Trait("Endpoint", "Identity")]
    public class IdentityEndpointsTests : IClassFixture<DimaFinanceiroApplicationFactory>
    {
        public DimaFinanceiroApplicationFactory _webApplicationFactory;

        private readonly Faker _faker = new("pt_BR");

        public IdentityEndpointsTests(DimaFinanceiroApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task POST_Register_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            RegisterRequest request = new()
            {
                Email = _faker.Person.Email,
                Password = "Teste1."
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("v1/identity/register", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task POST_Register_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            RegisterRequest request = new();
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("v1/identity/register", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_Login_WithValidData_ShouldReturn200StatusCodeAndSetCookiesCorrectly()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
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
                Password = registerRequest.Password
            };
            string loginBody = JsonSerializer.Serialize(loginRequest);
            StringContent loginStringContent = new(loginBody, Encoding.UTF8, "application/json");

            var loginResponse = await client.PostAsync("v1/identity/login?useCookies=true", loginStringContent);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            loginResponse.Headers.GetValues("Set-Cookie").Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public async Task POST_Login_WithInvalidData_ShouldReturn401StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
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
                Email = _faker.Person.FirstName,
                Password = "Teste2."
            };
            string loginBody = JsonSerializer.Serialize(loginRequest);
            StringContent loginStringContent = new(loginBody, Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("v1/identity/login", loginStringContent);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        }

        [Fact]
        public async Task GET_Roles_ShouldReturn200StatusCodeWithUserRoles()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            var getRolesResponse = await client.GetFromJsonAsync<RoleClaim[]>("v1/identity/roles");
            getRolesResponse.Should().NotBeNull();
        }

        [Fact]
        public async Task POST_Logout_ShouldReturn200StatusCodeAndSignOutTheUser()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            StringContent emptyJson = new("{}", Encoding.UTF8, "application/json");
            var logoutResponse = await client.PostAsync("v1/identity/logout", emptyJson);
            logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var testAuthenticationAfterLogoutResponse = await client.PostAsync("v1/identity/logout", emptyJson);
            testAuthenticationAfterLogoutResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
                Password = registerRequest.Password
            };
            string loginBody = JsonSerializer.Serialize(loginRequest);
            StringContent loginStringContent = new(loginBody, Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("v1/identity/login?useCookies=true", loginStringContent);
        }
    }
}
