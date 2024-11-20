using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Transactions
{
    public class GetByIdTransactionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("{id}", HandleAsync)
                .WithName("Transactions: Get by id")
                .WithSummary("Retorna uma transação por um id")
                .WithOrder(4)
                .Produces<Response<Transaction?>>();
        }

        private static async Task<IResult> HandleAsync(long id, ITransactionHandler handler, ClaimsPrincipal user)
        {
            var request = new GetByIdTransactionRequest
            {   
                UserId = user.Identity.Name,
                Id = id
            };

            var result = await handler.GetByIdAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.BadRequest(result);
        }
    }
}
