using Dima.API.Common;
using Dima.Core.Common;
using Dima.Core.Handlers;
using Dima.Core.Requests.Transactions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.API.Endpoints.Transactions
{
    public class GetByPeriodTransactionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/", HandleAsync)
                .WithName("Transactions: Get by period")
                .WithSummary("Retorna uma lista de transações de dentro de um período");
        }

        private static async Task<IResult> HandleAsync
            (
                ITransactionHandler handler,
                ClaimsPrincipal user,
                [FromQuery] int pageSize = Configuration.DefaultPageSize,
                [FromQuery] int pageNumber = 1,
                [FromQuery] DateTime? startDate = null,
                [FromQuery] DateTime? endDate = null
            )
        {
            var request = new GetByPeriodTransactionRequest
            {
                UserId = user.Identity.Name,
                PageNumber = pageNumber,
                PageSize = pageSize,
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await handler.GetByPeriodAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.BadRequest(result);

        }
    }
}
