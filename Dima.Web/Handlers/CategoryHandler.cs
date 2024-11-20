using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using Dima.Web.Common;
using System.Net.Http.Json;

namespace Dima.Web.Handlers
{
    public class CategoryHandler(IHttpClientFactory httpClientFactory) : ICategoryHandler
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

        public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
        {
            var result = await _client.PostAsJsonAsync("v1/categories",request);
            return await result.Content.ReadFromJsonAsync<Response<Category?>>()
                ?? new Response<Category?>(null, 400, "Houve uma falha ao criar sua categoria");
        }

        public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
        {
            var result = await _client.DeleteAsync($"v1/categories/{request.Id}");
            var response = new Response<Category?>(null, (int)result.StatusCode, "Categoria deletada com sucesso");
            var nullResponse = new Response<Category?>(null, (int)result.StatusCode, "Houve uma falha ao deletar sua categoria");
            return result.IsSuccessStatusCode ? response : nullResponse;
        } 

        public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoryRequest request)
        {
            var result = await _client.GetFromJsonAsync<PagedResponse<List<Category>?>>("v1/categories");
            return result
                ?? new PagedResponse<List<Category>?>(null, 400, "Não foi possível encontrar suas categorias");
        }

        public async Task<Response<Category?>> GetByIdAsync(GetByIdCategoryRequest request)
        {
            return await _client.GetFromJsonAsync<Response<Category?>>($"v1/categories/{request.Id}")
                ?? new Response<Category?>(null, 400, "Não foi possível encontrar sua categoria");
        }

        public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
        {
            var result = await _client.PutAsJsonAsync($"v1/categories/{request.Id}", request);
            return await result.Content.ReadFromJsonAsync<Response<Category?>>()
                ?? new Response<Category?>(null, 400, "Não foi possível atualizar sua categoria");
        }
    }
}
