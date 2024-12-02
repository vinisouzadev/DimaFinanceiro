using Bogus;
using Dima.API.IntegrationTests.ApplicationFactory;
using Dima.Core.Models;
using Dima.Core.Requests.Account;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Dima.API.IntegrationTests.EndpointGroups
{
    [Trait("Endpoint", "Transaction")]
    public class TransactionEndpointsTests : IClassFixture<DimaFinanceiroApplicationFactory>
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly DimaFinanceiroApplicationFactory _webApplicationFactory;

        public TransactionEndpointsTests(DimaFinanceiroApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        #region Tests

        #region Create

        [Fact]
        public async Task POST_Create_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);

            CreateTransactionRequest request = new()
            {
                 Title = _faker.Person.FirstName,
                 Amount = _faker.Random.Decimal(1,1900),
                 CategoryId = category.Id,
                 PaidOrReceivedAt = DateTime.UtcNow,
                 Type = Core.Enums.ETransactionType.Deposit,
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/transactions", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Create_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);

            CreateTransactionRequest request = new()
            {
                Title = _faker.Person.FirstName,
                Amount = _faker.Random.Decimal(1, 1900),
                CategoryId = -1,
                PaidOrReceivedAt = DateTime.UtcNow,
                Type = Core.Enums.ETransactionType.Deposit
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/transactions", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Create_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/transactions", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DELETE_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);
            Transaction transaction = await CreateTransaction(client, category.Id);

            var response = await client.DeleteAsync($"v1/transactions/{transaction.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task DELETE_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);
            Transaction transaction = await CreateTransaction(client, category.Id);

            var response = await client.DeleteAsync("v1/transactions/-1");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task DELETE_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            var response = await client.DeleteAsync("v1/transactions/1");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GET_GetbById_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);
            Transaction transaction = await CreateTransaction(client, category.Id);

            var response = await client.GetAsync($"v1/transactions/{transaction.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetById_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);
            Transaction transaction = await CreateTransaction(client, category.Id);

            var response = await client.GetAsync($"v1/transactions/-1");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetById_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            var response = await client.GetAsync($"v1/transactions/1");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region GetByPeriod

        [Fact]
        public async Task GET_GetByPeriod_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);
            Transaction transaction = await CreateTransaction(client, category.Id);
            GetByPeriodTransactionRequest request = new()
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            const string format = "yyyy-MM-dd";

            string startDate = request.StartDate.Value.ToString(format);
            string endDate = request.EndDate.Value.ToString(format);

            var response = await client.GetAsync($"v1/transactions?startDate={startDate}&endDate={endDate}");
            var responseData = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetByPeriod_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            const string format = "yyyy-MM-dd";
            string startDate = DateTime.UtcNow.ToString(format);
            string endDate = DateTime.UtcNow.ToString(format);

            var response = await client.GetAsync($"v1/transactions?startDate={startDate}&endDate={endDate}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region Update

        [Fact]
        public async Task PUT_Update_WithValidDate_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);
            Transaction transaction = await CreateTransaction(client, category.Id);

            UpdateTransactionRequest request = new()
            {
                Amount = _faker.Random.Decimal(1, 1400),
                CategoryId = category.Id,
                PaidOrReceivedAt = DateTime.UtcNow,
                Title = _faker.Vehicle.Model(),
                Type = Core.Enums.ETransactionType.Deposit
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"v1/transactions/{transaction.Id}", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task PUT_Update_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Category category = await CreateCategory(client);
            Transaction transaction = await CreateTransaction(client, category.Id);

            UpdateTransactionRequest request = new()
            {
                Amount = _faker.Random.Decimal(1, 1400),
                CategoryId = category.Id,
                PaidOrReceivedAt = DateTime.UtcNow,
                Title = _faker.Vehicle.Model(),
                Type = Core.Enums.ETransactionType.Deposit
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("v1/transactions/-1", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task PUT_Update_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            UpdateTransactionRequest request = new()
            {
                Amount = _faker.Random.Decimal(1, 1400),
                CategoryId = 1,
                PaidOrReceivedAt = DateTime.UtcNow,
                Title = _faker.Vehicle.Model(),
                Type = Core.Enums.ETransactionType.Deposit
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("v1/transactions/1", stringContent);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #endregion

        #region Private

        private async Task<Transaction> CreateTransaction(HttpClient client, long categoryId) 
        {
            CreateTransactionRequest request = new()
            {
                CategoryId = categoryId,
                Title = _faker.Vehicle.Model(),
                Amount = _faker.Random.Decimal(1, 1400),
                PaidOrReceivedAt = DateTime.UtcNow,
                Type = Core.Enums.ETransactionType.Deposit
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/transactions", stringContent);
            var responseData = await response.Content.ReadFromJsonAsync<Response<Transaction?>>();
            return responseData!.Data!;
        }

        private async Task<Category> CreateCategory(HttpClient client) 
        {
            CreateCategoryRequest request = new()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer()
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/categories", stringContent);
            var responseData = await response.Content.ReadFromJsonAsync<Response<Category?>>();

            return responseData!.Data!;
        }

        private async Task ClientAuthentication(HttpClient client)
        {
            RegisterRequest registerRequest = new()
            {
                Email = _faker.Person.Email,
                Password = "Teste0."
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

        #endregion
    }
}
