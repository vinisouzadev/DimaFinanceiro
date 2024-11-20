using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;

namespace Dima.Core.Handlers
{
    public interface IVoucherHandler
    {
        Task<Response<Voucher?>> GetVoucherByCodeAsync(GetVoucherByCodeRequest request);
    }
}
