using Dima.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Dima.API.Common
{
    public static class AppExtensions
    {
        public static WebApplication ConfigureDevEnvironment(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapSwagger();
                //.RequireAuthorization();

            return app;
        }

        public static WebApplication UseSecurity(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public static WebApplication UseHttpsRedirectionExtension(this WebApplication app)
        {
            app.UseHttpsRedirection();
            return app;
        }

    }
}
