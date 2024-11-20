using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Report
{
    public class GetIncomesAndExpensesEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/incomes-expenses", HandlerAsync)
               .Produces<Response<List<IncomesAndExpenses>>>();

        }

        private static async Task<IResult> HandlerAsync(ClaimsPrincipal user, IReportHandler handler)
        {
            var request = new GetIncomesAndExpensesRequest
            {
                UserId = user.Identity.Name
            };

            var result = await handler.GetIncomesAndExpensesAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
