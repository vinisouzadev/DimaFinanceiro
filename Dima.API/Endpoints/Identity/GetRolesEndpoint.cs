using Dima.API.Common;
using Dima.Core.Models.Account;
using System.Security.Claims;

namespace Dima.API.Endpoints.Identity
{
    public class GetRolesEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/roles", Handle)
                .WithName("Identity: Roles")
                .WithSummary("Retorna as roles do usuário");
        }

        private static IResult Handle(ClaimsPrincipal user)
        {
            if (user.Identity is null || !user.Identity.IsAuthenticated)
                return Results.Unauthorized();
            
            var identity = (ClaimsIdentity)user.Identity;

            var roles = identity.FindAll(identity.RoleClaimType).Select(c => new RoleClaim
            {
                Issuer = c.Issuer,
                OriginalIssuer = c.OriginalIssuer,
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType
            });

            return TypedResults.Json(roles);
        }
    }
}
