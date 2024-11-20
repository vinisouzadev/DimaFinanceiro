using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders
{
    public partial class DetailsPageCode : ComponentBase
    {
        #region Parameters

        [Parameter]
        public string Code { get; set; } = string.Empty;

        #endregion

        #region Propriedades

        public Order Order { get; set; } = null!;

        #endregion

        #region Dependências

        [Inject]
        public IOrderHandler Handler { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            var request = new GetOrderByCodeRequest
            {
                Code = Code
            };
            var result = await Handler.GetOrderByCodeAsync(request);

            if (result.IsSuccess)
                Order = result.Data!;
            else
                Snackbar.Add(result.Message, Severity.Error);

        }

        #endregion

        #region Refresh

        public void Refresh(Order order)
        {
            Order = order;
            StateHasChanged();
        }

        #endregion
    }
}

