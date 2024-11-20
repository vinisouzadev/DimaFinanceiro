using Dima.API.Common;
using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Dima.API.Endpoints.Categories
{
    public class DeleteCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapDelete("/{id}",HandleAsycn)
                .WithName("Categories: Delete") 
                .WithSummary("Deleta uma categoria")
                .WithOrder(3)
                .Produces<Response<Category>>();
        }

        private static async Task<IResult> HandleAsycn(long id, ICategoryHandler handler, ClaimsPrincipal user)
        {
            var request = new DeleteCategoryRequest()
            {
                UserId = user.Identity.Name,
                Id = id
            };

            var result = await handler.DeleteAsync(request);

            return result.IsSuccess
                ? TypedResults.NoContent()
                : TypedResults.BadRequest(result);
        }
    }
}
