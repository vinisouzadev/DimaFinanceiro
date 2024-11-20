using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;

namespace Dima.Core.Handlers
{
    public interface IOrderHandler
    {
        Task<Response<List<Order>?>> GetAllOrdersAsync(GetAllOrdersRequest request);

        Task<Response<Order?>> GetOrderByCodeAsync(GetOrderByCodeRequest request);

        Task<Response<Order?>> CancelOrderAsync(CancelOrderRequest request);

        Task<Response<Order?>> CreateOrderAsync(CreateOrderRequest request);

        Task<Response<Order?>> PayOrderAsync(PayOrderRequest request);

        Task<Response<Order?>> RefundOrderAsync(RefundOrderRequest request);
    }
}
