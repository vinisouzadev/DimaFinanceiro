using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages
{
    public partial class HomePageCode : ComponentBase
    {
        #region Dependências

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IReportHandler Handler { get; set; } = null!;
        #endregion

        #region Propriedades

        public bool ShowValues { get; set; } = false;

        public FinancialSummary? Summary { get; set; }

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            var request = new GetFinancialSummaryRequest();

            var result = await Handler.GetFinancialSummaryAsync(request);

            if (result.IsSuccess)
            {
                Summary = result.Data;
            }
        }

        #endregion

        #region Methods

        public void ToggleShowValue() => ShowValues = !ShowValues;

        #endregion
    }
}
