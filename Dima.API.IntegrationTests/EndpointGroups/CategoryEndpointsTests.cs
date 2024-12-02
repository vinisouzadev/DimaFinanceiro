using Bogus;
using Dima.API.IntegrationTests.ApplicationFactory;
using Dima.Core.Models;
using Dima.Core.Requests.Account;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using FluentAssertions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Dima.API.IntegrationTests.EndpointGroups
{
    [Trait("Endpoint", "Category")]
    public class CategoryEndpointsTests : IClassFixture<DimaFinanceiroApplicationFactory>
    {
        private readonly DimaFinanceiroApplicationFactory _webApplicationFactory;

        private readonly Faker _faker = new("pt_BR");

        public CategoryEndpointsTests(DimaFinanceiroApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        #region Categories

        #region POST

        [Fact]
        public async Task POST_Categories_CreateWithValidData_ShouldReturn201StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var request = new CreateCategoryRequest()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer()
            };
            var body = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/categories", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task POST_Categories_CreateWithInvalidData_ShouldReturn400StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var request = new CreateCategoryRequest();
            var body = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/categories", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_Categories_TryAccessWithoutAuthentication_ShouldReturn404StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            var request = new CreateCategoryRequest()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer()
            };
            var body = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("v1/categories", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region GET

        [Fact]
        public async Task GET_Categories_GetAll_WithTwoCategoriesInTheDatabase_ShouldReturn200StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            for (int i = 0; i < 2; i++)
            {
                var request = new CreateCategoryRequest()
                {
                    Title = _faker.Vehicle.Model(),
                    Description = _faker.Vehicle.Manufacturer()
                };
                var body = JsonSerializer.Serialize(request);
                var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
                var createResponse = await client.PostAsync("v1/categories", stringContent);
            }
            var response = await client.GetAsync("v1/categories");
            var value = await response.Content.ReadFromJsonAsync<PagedResponse<List<Category>?>>();
            value!.Data!.Count.Should().Be(2);

        }

        [Fact]
        public async Task GET_Categories_GetAll_TryAccessWithoutAuthentication_ShouldReturn404StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            var response = await client.GetAsync("v1/categories");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GET_Categories_GetById_WithCorrectlyId_ShouldReturn200StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var createCategoryRequest = new CreateCategoryRequest()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer()
            };
            var createCategoryBody = JsonSerializer.Serialize(createCategoryRequest);
            var createCategoryStringContent = new StringContent(createCategoryBody, Encoding.UTF8, "application/json");
            var createCategoryResponse = await client.PostAsync("v1/categories", createCategoryStringContent);
            var createCategoryResponseData = await createCategoryResponse.Content.ReadFromJsonAsync<Response<Category?>>();
            long categoryId = createCategoryResponseData!.Data!.Id;

            var getByIdResponse = await client.GetAsync($"v1/categories/{categoryId}");
            var getByIdResponseData = await getByIdResponse.Content.ReadFromJsonAsync<Response<Category?>>();
            getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getByIdResponseData!.Data!.Title.Should().Be(createCategoryRequest.Title);
        }

        [Fact]
        public async Task GET_Categories_GetById_WithInvalidId_ShouldReturn400StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var createdCategory = await CreateCategory(client);
            var response = await client.GetAsync($"v1/categories/{_faker.Random.Long(-10, -1)}");
            var responseData = await response.Content.ReadFromJsonAsync<Response<Category?>>();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_Categories_GetById_TryAccessWithoutAuthentication_ShouldReturn404StatusCode()
        {
            var client = _webApplicationFactory.CreateClient();
            var response = await client.GetAsync($"v1/categories/1");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region PUT

        [Fact]
        public async Task PUT_Categories_WithInvalidId_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var createdCategory = await CreateCategory(client);

            var request = new UpdateCategoryRequest()
            {
                Title = _faker.Person.FirstName,
                Description = _faker.Person.LastName
            };
            var body = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"v1/categories/{_faker.Random.Long(-1, -10)}", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PUT_Categories_TryAccessWithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            var request = new UpdateCategoryRequest()
            {
                Title = _faker.Person.FirstName,
                Description = _faker.Person.LastName
            };
            var body = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"v1/categories/{_faker.Random.Long(1, 10)}", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PUT_Categories_WithValidId_ShouldReturn200StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var createdCategory = await CreateCategory(client);

            UpdateCategoryRequest request = new()
            {
                Title = _faker.Person.FirstName,
                Description = _faker.Person.LastName
            };
            var body = JsonSerializer.Serialize(request);
            StringContent stringContent = new(body, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"v1/categories/{createdCategory.Data!.Id}", stringContent);
            var responseData = await response.Content.ReadFromJsonAsync<Response<Category?>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseData!.Data!.Title.Should().Be(request.Title);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task DELETE_Categories_WithInvalidId_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var createdCategory = await CreateCategory(client);

            var response = await client.DeleteAsync($"v1/categories/{_faker.Random.Long(-10, -1)}");
            var responseData = await response.Content.ReadFromJsonAsync<Response<Category?>>();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseData!.Data!.Should().BeNull();
            responseData.Message.Should().Be("Não foi possível identificar essa categoria");
        }

        [Fact]
        public async Task DELETE_Categories_WithoutAuthentication_ShouldReturn404StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();

            long anyId = 1;
            var response = await client.DeleteAsync($"v1/categories/{anyId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DELETE_Categories_WithValidId_ShouldReturn400StatusCode()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            await ClientAuthentication(client);
            var createdCategory = await CreateCategory(client);

            var response = await client.DeleteAsync($"v1/categories/{createdCategory.Data!.Id}");
            var getCategoryAfterDelete = await client.GetAsync($"v1/categories/{createdCategory.Data.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getCategoryAfterDelete.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion


        #endregion

        private async Task ClientAuthentication(HttpClient client)
        {

            var registerRequest = new RegisterRequest()
            {
                Email = _faker.Person.Email,
                Password = "SenhaTeste1."
            };
            var registerBody = JsonSerializer.Serialize(registerRequest);
            var registerStringContent = new StringContent(registerBody, Encoding.UTF8, "application/json");
            var registerResponse = await client.PostAsync("v1/identity/register", registerStringContent);

            var loginRequest = new LoginRequest()
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            };
            var loginBody = JsonSerializer.Serialize(loginRequest);
            var loginStringContent = new StringContent(loginBody, Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("v1/identity/login?useCookies=true", loginStringContent);
            var cookie = loginResponse.Headers.GetValues("Set-Cookie");
        }

        private async Task<Response<Category?>> CreateCategory(HttpClient client)
        {
            var createCategoryRequest = new CreateCategoryRequest()
            {
                Title = _faker.Vehicle.Model(),
                Description = _faker.Vehicle.Manufacturer()
            };
            var createCategoryBody = JsonSerializer.Serialize(createCategoryRequest);
            var createCategoryStringContent = new StringContent(createCategoryBody, Encoding.UTF8, "application/json");
            var createCategoryResponse = await client.PostAsync("v1/categories", createCategoryStringContent);
            var createCategoryResponseData = await createCategoryResponse.Content.ReadFromJsonAsync<Response<Category?>>();
            return createCategoryResponseData!;
        }
    }
}
