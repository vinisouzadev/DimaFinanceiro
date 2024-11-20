﻿using Dima.API.Common;
using Dima.Core.Handlers;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.API.Endpoints.Stripe
{
    public class CreateSessionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/session", HandleAsync)
                .Produces<Response<string?>>();
        }

        private static async Task<IResult> HandleAsync
            (
                ClaimsPrincipal user,
                IStripeHandler handler,
                CreateSessionRequest request
            )
        {
            request.UserId = user.Identity!.Name ?? string.Empty;

            var result = await handler.CreateSessionAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
