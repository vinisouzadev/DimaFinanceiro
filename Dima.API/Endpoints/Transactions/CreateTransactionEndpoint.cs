using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Transactions
{
    public class CreateTransactionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/", HandleAsync)
                .WithName("Transactions: Create")
                .WithSummary("Cria uma transação")
                .WithOrder(1)
                .Produces<Response<Transaction?>>();
        }

        private static async Task<IResult> HandleAsync(ITransactionHandler handler, CreateTransactionRequest request, ClaimsPrincipal user)
        {
            request.UserId = user.Identity.Name;
            var result = await handler.CreateAsync(request);

            return result.IsSuccess
                ? Results.Created($"{result.Data.Id}", result)
                : Results.BadRequest(result);
        }
    }
}
 