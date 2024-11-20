using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Dima.Web.Common;
using System.Net.Http.Json;

namespace Dima.Web.Handlers
{
    public class ProductHandler(IHttpClientFactory httpClientFactory) : IProductHandler
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

        public async Task<PagedResponse<List<Product>?>> GetAllProductsAsync(GetAllProductsRequest request)
        => await _client.GetFromJsonAsync<PagedResponse<List<Product>?>>("v1/products")
            ?? new PagedResponse<List<Product>?>(null, 400, "Não foi possível obter os produtos");

        public async Task<Response<Product?>> GetProductBySlugAsync(GetProductBySlugRequest request)
        => await _client.GetFromJsonAsync<Response<Product?>>($"v1/products/{request.Slug}")
            ?? new Response<Product?>(null, 400, "Falha ao obter o produto");
    }
}
