using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Dima.Web;
using MudBlazor.Services;
using Dima.Web.Common;
using Dima.Web.Handlers;
using Microsoft.AspNetCore.Components.Authorization;
using Dima.Web.Security;
using Dima.Core.Handlers;
using Dima.API.Handlers;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;

Configuration.StripePublicKey = builder.Configuration.GetValue<string>("StripePublicKey") ?? string.Empty;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<CookieHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

builder.Services.AddScoped(x => (ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddMudServices();

builder.Services.AddHttpClient(Configuration.HttpClientName, opt =>
{
    opt.BaseAddress = new Uri(Configuration.BackendUrl);
}).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddTransient<IAccountHandler, AccountHandler>();
builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
builder.Services.AddTransient<IReportHandler, ReportHandler>();
builder.Services.AddTransient<IOrderHandler, OrderHandler>();
builder.Services.AddTransient<IStripeHandler, StripeHandler>();
builder.Services.AddTransient<IProductHandler, ProductHandler>();
builder.Services.AddTransient<IVoucherHandler, VoucherHandler>();

builder.Services.AddLocalization();
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-br");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-br");


await builder.Build().RunAsync();
