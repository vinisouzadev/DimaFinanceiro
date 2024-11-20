using Dima.API.Common;
using Dima.API.Endpoints.Categories;
using Dima.API.Endpoints.Identity;
using Dima.API.Endpoints.Orders;
using Dima.API.Endpoints.Report;
using Dima.API.Endpoints.Stripe;
using Dima.API.Endpoints.Transactions;
using Dima.API.Models;


namespace Dima.API.Endpoints
{
    public static class Endpoint
    {
        public static void MapEndpoints(this WebApplication app) 
        {
            var endpoints = app.MapGroup("");

            endpoints
                .MapGroup("v1/categories")
                .WithTags("Categories")
                .RequireAuthorization()
                .MapEndpoint<CreateCategoryEndpoint>()
                .MapEndpoint<UpdateCategoryEndpoint>()
                .MapEndpoint<DeleteCategoryEndpoint>()
                .MapEndpoint<GetByIdCategoryEndpoint>()
                .MapEndpoint<GetAllCategoryEndpoint>();

            endpoints
                .MapGroup("v1/transactions")
                .WithTags("Transaction")
                .RequireAuthorization()
                .MapEndpoint<CreateTransactionEndpoint>()
                .MapEndpoint<UpdateTransactionEndpoint>()
                .MapEndpoint<DeleteTransactionEndpoint>()
                .MapEndpoint<GetByIdTransactionEndpoint>()
                .MapEndpoint<GetByPeriodTransactionEndpoint>();

            endpoints
                .MapGroup("v1/identity")
                .WithTags("Identity")
                .MapIdentityApi<User>();

            endpoints
                .MapGroup("v1/identity")
                .WithTags("Identity")
                .MapEndpoint<LogoutEndpoint>()
                .MapEndpoint<GetRolesEndpoint>();

            endpoints
                .MapGroup("v1/reports")
                .WithTags("Reports")
                .RequireAuthorization()
                .MapEndpoint<GetIncomesAndExpensesEndpoint>()
                .MapEndpoint<GetExpensesByCategoryEndpoint>()
                .MapEndpoint<GetIncomesByCategoryEndpoint>()
                .MapEndpoint<GetFinancialSummaryEndpoint>();

            endpoints.MapGroup("v1/products")
                .WithTags("Products")
                .RequireAuthorization()
                .MapEndpoint<GetProductBySlugEndpoint>()
                .MapEndpoint<GetAllProductsEndpoint>();

            endpoints.MapGroup("v1/vouchers")
                .WithTags("Vouchers")
                .RequireAuthorization()
                .MapEndpoint<GetVoucherByCodeEndpoint>();

            endpoints.MapGroup("v1/orders")
                .WithTags("Orders")
                .RequireAuthorization()
                .MapEndpoint<GetAllOrdersEndpoint>()
                .MapEndpoint<GetOrderByCodeEndpoint>()
                .MapEndpoint<CreateOrderEndpoint>()
                .MapEndpoint<PayOrderEndpoint>()
                .MapEndpoint<CancelOrderEndpoint>()
                .MapEndpoint<RefundOrderEndpoint>();

            endpoints.MapGroup("v1/payments/stripe")
                .WithTags("Payments - Stripe")
                .RequireAuthorization()
                .MapEndpoint<CreateSessionEndpoint>();
        }

        private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}
