using Dima.API.Data;
using Dima.API.Handlers;
using Dima.API.Models;
using Dima.Core.Common;
using Dima.Core.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Dima.API.Common
{
    public static class BuilderExtensions
    {
        public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
        {
            Configuration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

            Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;

            Configuration.FrontendUrl = builder.Configuration.GetValue<string>("FrontendUrl") ?? string.Empty;

            ApiConfiguration.StripeApIKey = builder.Configuration.GetValue<string>("StripeApiKey") ?? string.Empty;

            StripeConfiguration.ApiKey = ApiConfiguration.StripeApIKey;

            return builder;
        }

        public static WebApplicationBuilder AddDataContexts(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.ConnectionString);
            });

            return builder;
        }

        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(n => n.FullName);
            });

            return builder;
        }

        public static WebApplicationBuilder AddSecurity(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();

            builder.Services.AddAuthorization();

            builder.Services
                .AddIdentityCore<User>()
                .AddRoles<IdentityRole<long>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddApiEndpoints();

            return builder;
        }

        public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(ApiConfiguration.CorsPolicyName, policy =>
                {
                    policy
                    .WithOrigins(
                        [
                            Configuration.BackendUrl,
                            Configuration.FrontendUrl
                        ])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();        
                });
            });

            return builder;
        }

        public static WebApplicationBuilder AddDependencyInjection(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
            builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
            builder.Services.AddTransient<IReportHandler, ReportHandler>();
            builder.Services.AddTransient<IProductHandler, ProductHandler>();
            builder.Services.AddTransient<IVoucherHandler, VoucherHandler>();
            builder.Services.AddTransient<IOrderHandler, OrderHandler>();
            builder.Services.AddTransient<IStripeHandler, StripeHandler>();

            return builder;
        }
    }
}
