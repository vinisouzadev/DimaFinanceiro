using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Report
{
    public class GetFinancialSummaryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/financial-summary", HandleAsync)
                .Produces<Response<FinancialSummary>>();
        }

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, IReportHandler handler)
        {
            var request = new GetFinancialSummaryRequest
            {
                UserId = user.Identity.Name
            };

            var result = await handler.GetFinancialSummaryAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
