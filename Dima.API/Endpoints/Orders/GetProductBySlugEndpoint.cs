using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;

namespace Dima.API.Endpoints.Orders
{
    public class GetProductBySlugEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/{slug}", HandleAsync)
                .WithName("Products: Get by Slug")
                .WithSummary("Recupera um produto pelo slug")
                .WithDescription("Recupera um produto pelo slug")
                .Produces<Response<Product?>>();
        }

        private static async Task<IResult> HandleAsync(IProductHandler handler, string slug)
        {
            var request = new GetProductBySlugRequest
            {
                Slug = slug
            };

            var result = await handler.GetProductBySlugAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
