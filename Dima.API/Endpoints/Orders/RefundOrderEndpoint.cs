using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Orders
{
    public class RefundOrderEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/{id}/refund", HandleAsync)
                .WithName("Orders: Refund")
                .WithSummary("Estorna o pagamento do pedido")
                .WithDescription("Estorna o pagamento do pedido")
                .Produces<Response<Order?>>();
        }

        private static async Task<IResult> HandleAsync(IOrderHandler handler, long id, ClaimsPrincipal user)
        {
            var request = new RefundOrderRequest
            {
                Id = id,
                UserId = user.Identity!.Name ?? string.Empty
            };

            var result = await handler.RefundOrderAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
