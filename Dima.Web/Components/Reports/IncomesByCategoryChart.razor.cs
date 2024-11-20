using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports
{
    public partial class IncomesByCategoryChartComponent : ComponentBase
    {
        #region Dependências

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IReportHandler Handler { get; set; } = null!;

        #endregion

        #region Propriedades

        public List<double> Data { get; set; } = [];

        public List<string> Labels { get; set; } = [];

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            var request = new GetIncomesByCategoryRequest();

            var result = await Handler.GetIncomesByCategoryAsync(request);

            if(!result.IsSuccess || result.Data is null)
            {
                Snackbar.Add("Houve uma falha ao buscar seu relatório", Severity.Error);
                return;
            }

            foreach(var item in result.Data)
            {
                Labels.Add($"{item.Category} ({item.Incomes:C})");
                Data.Add((double)item.Incomes);
            }
        }

        #endregion
    }
}
