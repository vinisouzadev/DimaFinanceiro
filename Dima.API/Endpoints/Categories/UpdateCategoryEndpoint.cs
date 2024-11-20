using Dima.API.Common;
using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Categories
{
    public class UpdateCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPut("/{id}", HandleAsync)
                .WithName("Categories: Update")
                .WithSummary("Atualiza uma categoria")
                .WithOrder(2)
                .Produces<Response<Category>>();
        }

        private static async Task<IResult> HandleAsync(long id, UpdateCategoryRequest request, ICategoryHandler handler, ClaimsPrincipal user)
        {
            request.UserId = user.Identity.Name;
            request.Id = id;
            
            var result = await handler.UpdateAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : Results.BadRequest(result);

        } 
    }
}
