using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Products
{
    public partial class ListProductsPageCode : ComponentBase
    {
        #region Propriedades

        public bool IsBusy { get; set; } = false;

        public List<Product> Products { get; set; } = [];

        #endregion

        #region Dependências

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IProductHandler Handler { get; set; } = null!;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;

            try
            {
                var request = new GetAllProductsRequest();
                var result = await Handler.GetAllProductsAsync(request);

                if (result.IsSuccess)
                {
                    Products = result.Data ?? [];
                    Snackbar.Add(result.Message, Severity.Success);
                }
                else
                {
                    Snackbar.Add(result.Message, Severity.Error);
                }

            }
            catch(Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
