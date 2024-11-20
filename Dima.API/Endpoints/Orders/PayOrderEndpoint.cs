using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Orders
{
    public class PayOrderEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/{orderNumber}/pay", HandleAsync)
                .WithName("Orders: Pay")
                .WithSummary("Realiza o pagamento do pedido")
                .WithDescription("Realiza o pagamento do pedido")
                .Produces<Response<Order?>>();
        }

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, string orderNumber, IOrderHandler handler)
        {
            var request = new PayOrderRequest
            {
                OrderNumber = orderNumber,
                UserId = user.Identity!.Name ?? string.Empty
            };

            var result = await handler.PayOrderAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
