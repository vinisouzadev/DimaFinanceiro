using Dima.API.Common;
using Dima.Core.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.API.Endpoints.Orders
{
    public class GetAllOrdersEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/", HandleAsync)
                .WithName("Orders: Get all")
                .WithSummary("Recupera todos os pedidos")
                .WithDescription("Recupera todos os pedidos")
                .Produces<PagedResponse<List<Order>?>>();
        }

        private static async Task<IResult> HandleAsync
            (IOrderHandler handler,
            ClaimsPrincipal user,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = Configuration.DefaultPageSize)
        {
            var request = new GetAllOrdersRequest
            {
                UserId = user.Identity!.Name ?? string.Empty,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await handler.GetAllOrdersAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
