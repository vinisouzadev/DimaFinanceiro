using Dima.API.Common;
using Dima.API.Handlers;
using Dima.Core.Requests.Categories;
using System.Security.Claims;
using Dima.Core.Models;
using Dima.Core.Responses;

namespace Dima.API.Endpoints.Categories
{
    public class GetByIdCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/{id}", HandleAsync)
                .WithName("Categories: Get by id")
                .WithSummary("Retorna uma categoria por id");
        }

        private static async Task<IResult> HandleAsync(long id, ICategoryHandler handler, ClaimsPrincipal user)
        {
            var request = new GetByIdCategoryRequest
            {
                UserId = user.Identity.Name,
                Id = id
            };

            var result = await handler.GetByIdAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
