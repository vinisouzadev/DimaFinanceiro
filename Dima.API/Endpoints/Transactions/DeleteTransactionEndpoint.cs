using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Transactions
{
    public class DeleteTransactionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapDelete("{id}", HandleAsync)
                .WithName("Transactions: Delete")
                .WithSummary("Deleta uma transação")
                .WithOrder(3)
                .Produces<Response<Transaction?>>();
        }

        private static async Task<IResult> HandleAsync(long id, ITransactionHandler handler, ClaimsPrincipal user)
        {
            var request = new DeleteTransactionRequest
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
