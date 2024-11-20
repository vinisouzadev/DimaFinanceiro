using Dima.API.Common;
using Dima.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Dima.API.Endpoints.Identity
{
    public class LogoutEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app
                .MapPost("/logout", HandleAsync)
                .WithName("Identity: Logout")
                .WithSummary("Realiza o logout do usuário")
                .RequireAuthorization();
        }

        private static async Task<IResult> HandleAsync(SignInManager<User> signInManager)
        {
            await signInManager.SignOutAsync();

            return Results.Ok();
        }
    }
}
