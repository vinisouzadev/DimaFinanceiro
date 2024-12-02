using Bogus;
using Dima.API.Data;
using Dima.API.IntegrationTests.ApplicationFactory;
using Dima.Core.Enums;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Account;
using Dima.Core.Requests.Order;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using Dima.Core.Responses.Stripe;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Dima.API.IntegrationTests.EndpointGroups
{
    [Trait("Endpoint", "Order")]
    public class OrderEndpointsTests : IClassFixture<DimaFinanceiroApplicationFactory>
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly DimaFinanceiroApplicationFactory _webApplicationFactory;

        public OrderEndpointsTests(DimaFinanceiroApplicationFactory webApplicationFactory)
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

            Voucher voucher = CreateVoucher();
            Product product = CreateProduct();

            CreateOrderRequest request = new()
            {
                ProductId = product.Id,
                VoucherId = voucher.Id
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/orders", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Create_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Voucher voucher = CreateVoucher();
            Product product = CreateProduct();

            CreateOrderRequest request = new()
            {
                ProductId = _faker.Random.Long(-1, -10),
                VoucherId = _faker.Random.Long(-1, -10)
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/orders", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Create_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            CreateOrderRequest request = new()
            {
                ProductId = 1,
                VoucherId = 1
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/orders", stringContent);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region Cancel

        [Fact]
        public async Task POST_Cancel_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Voucher voucher = CreateVoucher();
            Product product = CreateProduct();
            Order order = await CreateOrder(client, product.Id, voucher.Id);

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"v1/orders/{order.Id}/cancel", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Cancel_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            Voucher voucher = CreateVoucher();
            Product product = CreateProduct();
            Order order = await CreateOrder(client, product.Id, voucher.Id);

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"v1/orders/{-1}/cancel", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Cancel_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"v1/orders/{1}/cancel", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task GET_GetAll_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Voucher voucher = CreateVoucher();
            Voucher secondVoucher = CreateVoucher();
            Product product = CreateProduct();
            Order firstOrder = await CreateOrder(client, product.Id, voucher.Id);
            Order secondOrder = await CreateOrder(client, product.Id, secondVoucher.Id);

            var response = await client.GetAsync("v1/orders");
            var responseData = await response.Content.ReadFromJsonAsync<PagedResponse<List<Order>>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseData!.Data.Should().NotBeEmpty().And.NotBeNull();
            responseData.Data!.Count.Should().Be(2);
            responseData.Data.Any(order => order.OrderCode == firstOrder.OrderCode).Should().BeTrue();
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetAll_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            var response = await client.GetAsync("v1/orders");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region GetByCode

        [Fact]
        public async Task GET_GetByCode_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Voucher voucher = CreateVoucher();
            Product product = CreateProduct();
            Order order = await CreateOrder(client, product.Id, voucher.Id);

            var response = await client.GetAsync($"v1/orders/{order.OrderCode}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetByCode_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Voucher voucher = CreateVoucher();
            Product product = CreateProduct();
            Order order = await CreateOrder(client, product.Id, voucher.Id);

            var response = await client.GetAsync($"v1/orders/teste0");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task GET_GetByCode_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            var response = await client.GetAsync("v1/orders/teste0");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region Pay

        [Fact]
        public async Task POST_Pay_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            
            await ClientAuthentication(client);

            Product product = CreateProduct();
            Voucher voucher = CreateVoucher();
            Order order = await CreateOrder(client, product.Id, voucher.Id);

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsJsonAsync($"v1/orders/{order.OrderCode}/pay", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Pay_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Product product = CreateProduct();
            Voucher voucher = CreateVoucher();
            Order order = await CreateOrder(client, product.Id, voucher.Id);

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/orders/{teste0}/pay", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Pay_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/orders/teste0/pay",stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #region Refund

        [Fact]
        public async Task POST_Refund_WithValidData_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Product product = CreateProduct();
            Voucher voucher = CreateVoucher();
            Order createdOrder = await CreateOrder(client, product.Id, voucher.Id);

            using var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var updatedOrder = context.Orders.SingleOrDefault(order => order.OrderCode == createdOrder.OrderCode);
            updatedOrder!.Status = EOrderStatus.Paid;
            context.SaveChanges();

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"v1/orders/{createdOrder.Id}/refund", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Refund_WithInvalidData_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);

            Product product = CreateProduct();
            Voucher voucher = CreateVoucher();
            Order order = await CreateOrder(client, product.Id, voucher.Id);

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"v1/orders/teste0/refund",stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        [Fact]
        public async Task POST_Refund_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            StringContent stringContent = new("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/orders/teste0/refund", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await _webApplicationFactory.DatabaseClearAsync();
        }

        #endregion

        #endregion

        #region Private

        private async Task<Order> CreateOrder(HttpClient client, long productId, long voucherId)
        {
            CreateOrderRequest request = new()
            {
                ProductId = productId,
                VoucherId = voucherId
            };
            string body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/orders", stringContent);
            var responseData = await response.Content.ReadFromJsonAsync<Response<Order?>>();
            return responseData!.Data!;
        }

        private Voucher CreateVoucher()
        {
            using IServiceScope scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Voucher voucher = new()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Vin(),
                Amount = _faker.Random.Decimal(1, 50),
                IsActive = true,
                VourcherCode = _faker.Random.Hexadecimal(5)
            };
            context.Vouchers.Add(voucher);
            context.SaveChanges();
            return voucher;
        }

        private Product CreateProduct()
        {
            using IServiceScope scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Product product = new()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer(),
                IsActive = true,
                Price = _faker.Random.Decimal(100, 1000),
                Slug = _faker.Random.Hexadecimal(5)
            };
            context.Products.Add(product);
            context.SaveChanges();
            return product;
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

        #endregion
    }
}
