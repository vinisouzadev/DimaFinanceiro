using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Orders
{
    public class GetOrderByCodeEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/{code}", HandleAsync)
                .WithName("Orders: Get by code")
                .WithSummary("Recupera um pedido pelo seu código")
                .WithDescription("Recupera um pedido pelo seu código")
                .Produces<Response<Order?>>();
        }

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, IOrderHandler handler, string code)
        {
            var request = new GetOrderByCodeRequest
            {
                UserId = user.Identity!.Name ?? string.Empty,
                Code = code
            };

            var result = await handler.GetOrderByCodeAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
