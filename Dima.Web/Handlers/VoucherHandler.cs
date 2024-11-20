using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Dima.Web.Common;
using System.Net.Http.Json;

namespace Dima.Web.Handlers
{
    public class VoucherHandler(IHttpClientFactory httpClientFactory) : IVoucherHandler
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

        public async Task<Response<Voucher?>> GetVoucherByCodeAsync(GetVoucherByCodeRequest request)
        => await _client.GetFromJsonAsync<Response<Voucher?>>($"v1/vouchers/{request.Code}")
            ?? new Response<Voucher?>(null, 400, "Não foi possível obter o voucher");
            
    }
}
