using Dima.API.Common;
using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Categories
{
    public class CreateCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/", HandleAsync)
                .WithName("Categories: Create")
                .WithSummary("Cria uma categoria")
                .WithOrder(1)
                .Produces<Response<Category>>();
    
        private static async Task<IResult> HandleAsync(ICategoryHandler handler, CreateCategoryRequest request, ClaimsPrincipal user)
        {
            request.UserId = user.Identity.Name;

            var result = await handler.CreateAsync(request);

            return result.IsSuccess
                ? Results.Created($"/{result.Data.Id}", result)
                : Results.BadRequest(result);
        }
    }


}
