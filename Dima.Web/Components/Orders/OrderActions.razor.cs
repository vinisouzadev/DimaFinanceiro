using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Requests.Stripe;
using Dima.Web.Common;
using Dima.Web.Pages.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Dima.Web.Components.Orders
{
    public partial class OrderActionsComponent : ComponentBase
    {

        #region Parameters

        [Parameter, EditorRequired]
        public Order Order { get; set; } = null!;

        [CascadingParameter]
        public DetailsPageCode Parent { get; set; } = null!;

        #endregion

        #region Dependências

        [Inject]
        public IJSRuntime JsRuntime { get; set; } = null!;

        [Inject]
        public IDialogService DialogService { get; set; } = null!;

        [Inject]
        public IOrderHandler OrderHandler { get; set; } = null!;

        [Inject]
        public IStripeHandler StripeHandler { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        #endregion

        #region Public

        public async Task OnCancelButtonClickedAsync()
        {
            var result = await DialogService.ShowMessageBox("ATENÇÃO", "Deseja realmente cancelar esse pedido?", yesText: "SIM", cancelText: "NÃO");

            if (result is not null && result == true)
                await CancelOrderAsync();
        }

        public async Task OnPayButtonClikedAsync()
        {
            await PayOrderAsync();
        }

        public async Task OnRefundButtonClickedAsync()
        {
            var result = await DialogService.ShowMessageBox("ATENÇÃO", "Deseja realmente estornar esse pedido?", yesText: "SIM", cancelText: "NÃO");

            if (result is not null && result == true)
                await RefundOrderAsync();
        }

        #endregion

        #region Private

        private async Task CancelOrderAsync()
        {
            var request = new CancelOrderRequest
            {
                Id = Order.Id
            };

            var result = await OrderHandler.CancelOrderAsync(request);

            if (result.IsSuccess)
            {
                Parent.Refresh(result.Data!);
            }

            else
                Snackbar.Add(result.Message, Severity.Error);

        }

        private async Task PayOrderAsync()
        {
            CreateSessionRequest request = new()
            {
                OrderNumber = Order.OrderCode,
                OrderTotal = (long)Math.Round(Order.Total * 100, 2),
                ProductTitle = Order.Product.Title,
                ProductDescription = Order.Product.Description
            };

            try
            {
                var result = await StripeHandler.CreateSessionAsync(request);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Error);
                    return;
                }
                if (result.Data is null)
                {
                    Snackbar.Add(result.Message, Severity.Error);
                }

                await JsRuntime.InvokeVoidAsync("checkout", Configuration.StripePublicKey, result.Data);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Falha ao gerar sessão no Stripe", Severity.Error);
            }
        }

        private async Task RefundOrderAsync()
        {
            var request = new RefundOrderRequest
            {
                Id = Order.Id
            };

            var result = await OrderHandler.RefundOrderAsync(request);

            if (result.IsSuccess)
            {
                Parent.Refresh(result.Data!);
            }

            else
                Snackbar.Add(result.Message, Severity.Error);

        }

        #endregion
    }
}
