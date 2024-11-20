using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;

namespace Dima.API.Endpoints.Orders
{
    public class GetVoucherByCodeEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/{code}", HandleAsync)
                .WithName("Vouchers: Get by Code")
                .WithSummary("Recupera um voucher pelo seu código")
                .WithDescription("Recupera um voucher pelo seu código")
                .Produces<Response<Voucher?>>();
        }

        private static async Task<IResult> HandleAsync(IVoucherHandler handler, string code)
        {
            var request = new GetVoucherByCodeRequest
            {
                Code = code
            };

            var result = await handler.GetVoucherByCodeAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
