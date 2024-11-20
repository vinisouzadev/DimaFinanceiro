using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Orders
{
    public class CreateOrderEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/", HandleAsync)
                .WithName("Orders: Create")
                .WithSummary("Cria um pedido")
                .WithDescription("Cria um pedido")
                .Produces<Response<Order?>>();
        }

        private static async Task<IResult> HandleAsync(IOrderHandler handler, CreateOrderRequest request, ClaimsPrincipal user)
        {
            request.UserId = user.Identity.Name ?? string.Empty;

            var result = await handler.CreateOrderAsync(request);

            return result.IsSuccess
                ? TypedResults.Created($"v1/orders/{result.Data?.OrderCode}", result)
                : TypedResults.BadRequest(result);
        }
    }
}
