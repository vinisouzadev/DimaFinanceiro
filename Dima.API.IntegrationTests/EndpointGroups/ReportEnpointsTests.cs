using Bogus;
using Dima.API.Data;
using Dima.API.IntegrationTests.ApplicationFactory;
using Dima.Core.Enums;
using Dima.Core.Models;
using Dima.Core.Requests.Account;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Dima.API.IntegrationTests.EndpointGroups
{
    [Trait("Endpoint", "Report")]
    public class ReportEnpointsTests : IClassFixture<DimaFinanceiroApplicationFactory>
    {
        private readonly DimaFinanceiroApplicationFactory _webApplicationFactory;

        private readonly Faker _faker = new("pt_BR");

        public ReportEnpointsTests(DimaFinanceiroApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task GET_GetExpensesByCategory_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            using var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.ExecuteSqlRaw("CREATE OR REPLACE VIEW public.\"vwGetExpensesByCategory\"\r\n" +
                "AS SELECT t.\"UserId\",\r\n" +
                "    c.\"Title\" AS \"Category\",\r\n" +
                "    EXTRACT(year FROM t.\"PaidOrReceivedAt\") AS \"Year\",\r\n" +
                "    sum(t.\"Amount\") AS \"Expenses\"\r\n" +
                "   FROM \"Transaction\" t\r\n" +
                "     JOIN \"Category\" c ON c.\"Id\" = t.\"CategoryId\"\r\n" +
                "  WHERE t.\"PaidOrReceivedAt\" >= (CURRENT_DATE + '-11 mons'::interval) AND t.\"PaidOrReceivedAt\" < (CURRENT_DATE + '1 mon'::interval) AND t.\"Type\" = 2\r\n" +
                "  GROUP BY t.\"UserId\", c.\"Title\", (EXTRACT(year FROM t.\"PaidOrReceivedAt\"));");

            await ClientAuthenticationAsync(client);

            Category category = await CreateCategoryAsync(client);
            Transaction transaction = await CreateTransactionAsync(client, category.Id, ETransactionType.Withdraw);

            var response = await client.GetAsync("v1/reports/expenses");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetExpensesByCategory_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            var response = await client.GetAsync("v1/reports/expenses");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GET_FinancialSummary_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthenticationAsync(client);

            Category category = await CreateCategoryAsync(client);
            Transaction depositTransaction = await CreateTransactionAsync(client, category.Id, ETransactionType.Deposit);
            Transaction withdrawTransaction = await CreateTransactionAsync(client, category.Id, ETransactionType.Withdraw);

            var response = await client.GetAsync("v1/reports/financial-summary");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_FinancialSummary_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            var response = await client.GetAsync("v1/reports/financial-summary");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_IncomesAndExpenses_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            using var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.ExecuteSqlRaw("CREATE OR REPLACE VIEW public.\"vwGetIncomesAndExpenses\"\r\n" +
                "AS SELECT t.\"UserId\",\r\n" +
                "    EXTRACT(month FROM t.\"PaidOrReceivedAt\") AS \"Month\",\r\n" +
                "    EXTRACT(year FROM t.\"PaidOrReceivedAt\") AS \"Year\",\r\n" +
                "    sum(\r\n" +
                "        CASE\r\n" +
                "            WHEN t.\"Type\" = 1 THEN t.\"Amount\"\r\n" +
                "            ELSE 0::money\r\n        END) AS \"Incomes\",\r\n" +
                "    sum(\r\n        CASE\r\n" +
                "            WHEN t.\"Type\" = 2 THEN t.\"Amount\"\r\n" +
                "            ELSE 0::money\r\n        END) AS \"Expenses\"\r\n" +
                "   FROM \"Transaction\" t\r\n" +
                "     JOIN \"Category\" c ON c.\"Id\" = t.\"CategoryId\"\r\n" +
                "  WHERE t.\"PaidOrReceivedAt\" >= (CURRENT_DATE + '-11 mons'::interval) AND t.\"PaidOrReceivedAt\" < (CURRENT_DATE + '1 mon'::interval)\r\n" +
                "  GROUP BY t.\"UserId\", (EXTRACT(year FROM t.\"PaidOrReceivedAt\")), (EXTRACT(month FROM t.\"PaidOrReceivedAt\"));");
            await ClientAuthenticationAsync(client);

            Category category = await CreateCategoryAsync(client);
            Transaction depositTransaction = await CreateTransactionAsync(client, category.Id, ETransactionType.Deposit);
            Transaction withdrawTransaction = await CreateTransactionAsync(client, category.Id, ETransactionType.Withdraw);

            var response = await client.GetAsync("v1/reports/incomes-expenses");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_IncomesAndExpenses_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            var response = await client.GetAsync("v1/reports/incomes-expenses");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GET_IncomesByCategory_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            using var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.ExecuteSqlRaw("CREATE OR REPLACE VIEW public.\"vwGetIncomesByCategory\"\r\n" +
                "AS SELECT t.\"UserId\",\r\n" +
                "    c.\"Title\" AS \"Category\",\r\n" +
                "    EXTRACT(year FROM t.\"PaidOrReceivedAt\") AS \"Year\",\r\n" +
                "    sum(t.\"Amount\"::numeric) AS \"Incomes\"\r\n" +
                "   FROM \"Transaction\" t\r\n" +
                "     JOIN \"Category\" c ON c.\"Id\" = t.\"CategoryId\"\r\n" +
                "  WHERE t.\"PaidOrReceivedAt\" >= (CURRENT_DATE + '-11 mons'::interval) AND t.\"PaidOrReceivedAt\" < (CURRENT_DATE + '1 mon'::interval) AND t.\"Type\" = 1\r\n" +
                "  GROUP BY t.\"UserId\", c.\"Title\", (EXTRACT(year FROM t.\"PaidOrReceivedAt\"));");
            await ClientAuthenticationAsync(client);

            Category category = await CreateCategoryAsync(client);
            Transaction transaction = await CreateTransactionAsync(client, category.Id, ETransactionType.Deposit);

            var response = await client.GetAsync("v1/reports/incomes");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_IncomesByCategory_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            var response = await client.GetAsync("v1/reports/incomes");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #region Private

        private async Task<Transaction> CreateTransactionAsync(HttpClient client, long categoryId, ETransactionType type)
        {
            CreateTransactionRequest request = new()
            {
                CategoryId = categoryId,
                Title = _faker.Vehicle.Fuel(),
                Amount = _faker.Random.Decimal(1, 1400),
                PaidOrReceivedAt = DateTime.UtcNow,
                Type = type
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/transactions", stringContent);
            var responseData = await response.Content.ReadFromJsonAsync<Response<Transaction>>();
            return responseData!.Data!;
        }

        private async Task<Category> CreateCategoryAsync(HttpClient client)
        {
            CreateCategoryRequest request = new()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer()
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/categories", stringContent);
            var responseData = await response.Content.ReadFromJsonAsync<Response<Category>>();
            return responseData!.Data!;
        }

        private async Task ClientAuthenticationAsync(HttpClient client)
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
