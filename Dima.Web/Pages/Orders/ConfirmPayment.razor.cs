using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders
{
    public partial class ConfirmPaymentPageCode : ComponentBase
    {
        #region Parameters

        [Parameter]
        public string OrderNumber { get; set; } = string.Empty;

        #endregion

        #region Services

        [Inject]
        public IOrderHandler OrderHandler { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        #endregion

        #region Properties

        public Order? Order { get; set; }

        #endregion

        #region Overrides

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            PayOrderRequest request = new()
            {
                OrderNumber = OrderNumber
            };

            try
            {
                var result = await OrderHandler.PayOrderAsync(request);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Error);
                    return;
                }

                Order = result.Data;
                Snackbar.Add(result.Message, Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }
        #endregion
    }
}
