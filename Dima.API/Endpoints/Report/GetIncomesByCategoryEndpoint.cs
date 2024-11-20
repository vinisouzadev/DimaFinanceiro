using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Report
{
    public class GetIncomesByCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/incomes", HandleAsync)
                .Produces<Response<List<IncomesByCategory>>>();
        }

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, IReportHandler handler)
        {
            var request = new GetIncomesByCategoryRequest
            {
                UserId = user.Identity.Name
            };

            var result = await handler.GetIncomesByCategoryAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
