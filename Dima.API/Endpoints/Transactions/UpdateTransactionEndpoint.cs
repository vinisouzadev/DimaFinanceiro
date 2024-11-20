using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Transactions
{
    public class UpdateTransactionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPut("/{id}", HandleAsync)
                .WithName("Transactions: Update")
                .WithSummary("Atualiza uma transação")
                .WithOrder(2)
                .Produces<Response<Transaction?>>() ;
        }

        private static async Task<IResult> HandleAsync(long id, ITransactionHandler handler, UpdateTransactionRequest request, ClaimsPrincipal user)
        {
            request.UserId = user.Identity.Name;
            request.Id = id;

            var result = await handler.UpdateAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.BadRequest(result);
        }
    }
}
