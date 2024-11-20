using Dima.API.Common;
using Dima.Core.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dima.API.Endpoints.Orders
{
    public class GetAllProductsEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/", HandleAsync)
                .WithName("Products: Get all")
                .WithSummary("Recupera todos os produtos")
                .WithDescription("Recupera todos os produtos")
                .Produces<PagedResponse<List<Order>?>>();
        }

        private static async Task<IResult> HandleAsync
            (IProductHandler handler,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = Configuration.DefaultPageSize)
        {
            var request = new GetAllProductsRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await handler.GetAllProductsAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);

        }
    }
}
