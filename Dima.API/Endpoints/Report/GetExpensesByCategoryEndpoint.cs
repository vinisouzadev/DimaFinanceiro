using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Report
{
    public class GetExpensesByCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/expenses", HandleAsync)
                .Produces<Response<ExpensesByCategory>>();
        }

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, IReportHandler handler)
        {
            var request = new GetExpensesByCategoryRequest
            {
                UserId = user.Identity.Name
            };

            var result = await handler.GetExpensesByCategoryAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
