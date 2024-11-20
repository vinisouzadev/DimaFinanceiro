using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Dima.Web.Common;
using System.Net.Http.Json;

namespace Dima.Web.Handlers
{
    public class OrderHandler(IHttpClientFactory httpClientFactory) : IOrderHandler
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

        public async Task<Response<Order?>> CancelOrderAsync(CancelOrderRequest request)
        {
            var result = await _client.PostAsJsonAsync($"v1/orders/{request.Id}/cancel", request);
            return await result.Content.ReadFromJsonAsync<Response<Order?>>()
                ?? new Response<Order?>(null, 400, "Não foi possível cancelar o pedido");
        }

        public async Task<Response<Order?>> CreateOrderAsync(CreateOrderRequest request)
        {
            var result = await _client.PostAsJsonAsync("v1/orders", request);
            return await result.Content.ReadFromJsonAsync<Response<Order?>>()
                ?? new Response<Order?>(null, 400, "Não foi possível criar o pedido");
        }

        public async Task<Response<List<Order>?>> GetAllOrdersAsync(GetAllOrdersRequest request)
            => await _client.GetFromJsonAsync<PagedResponse<List<Order>?>>("v1/orders")
            ?? new PagedResponse<List<Order>?>(null, 400, "Não foi possível obter todos os pedidos");

        public async Task<Response<Order?>> GetOrderByCodeAsync(GetOrderByCodeRequest request)
            => await _client.GetFromJsonAsync<Response<Order?>>($"v1/orders/{request.Code}")
            ?? new Response<Order?>(null, 400, "Não foi possível obter o pedido");

        public async Task<Response<Order?>> PayOrderAsync(PayOrderRequest request)
        {
            var result = await _client.PostAsJsonAsync($"v1/orders/{request.OrderNumber}/pay", request);

            return await result.Content.ReadFromJsonAsync<Response<Order?>>()
                ?? new Response<Order?>(null, 400, "Não foi possível realizar o pagamento do pedido");
        }

        public async Task<Response<Order?>> RefundOrderAsync(RefundOrderRequest request)
        {
            var result = await _client.PostAsJsonAsync($"v1/orders/{request.Id}/refund", request);

            return await result.Content.ReadFromJsonAsync<Response<Order?>>()
                ?? new Response<Order?>(null, 400, "Não foi possível realizar o estorno do pagamento");
        }
    }
}
