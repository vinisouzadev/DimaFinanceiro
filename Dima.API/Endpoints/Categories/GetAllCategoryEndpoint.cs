using Dima.API.Common;
using Dima.API.Handlers;
using Dima.Core.Common;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Dima.API.Endpoints.Categories
{
    public class GetAllCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/", HandleAsync)
                .WithName("Categories: Get all")
                .WithSummary("Retorna todas as categorias")
                .WithOrder(5)
                .Produces<PagedResponse<List<Category?>>>();
        }
    
        private static async Task<IResult> HandleAsync
            (
                ICategoryHandler handler,
                ClaimsPrincipal user,
                [FromQuery]int pageSize = Configuration.DefaultPageSize,
                [FromQuery]int pageNumber = 1)
            {
            var request = new GetAllCategoryRequest();

            request.UserId = user.Identity.Name;
            request.PageSize = pageSize;
            request.PageNumber = pageNumber;

            var result = await handler.GetAllAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.BadRequest(result);

        }
    }
}
