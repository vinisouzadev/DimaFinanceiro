using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports
{
    public partial class IncomesAndExpensesChartComponent : ComponentBase
    {
        #region Dependências

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IReportHandler Handler { get; set; } = null!;

        #endregion

        #region Propriedads

        public List<ChartSeries> Series { get; set; } = [];

        public List<string> Labels { get; set; } = [];

        public ChartOptions Options { get; set; } = new();

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            var request = new GetIncomesAndExpensesRequest();

            var result = await Handler.GetIncomesAndExpensesAsync(request);

            if(!result.IsSuccess || result.Data is null)
            {
                Snackbar.Add("Houve uma falha para exibir seu histórico", Severity.Error);
                return;
            }

            var incomes = new List<double>();
            var expenses = new List<double>();

            foreach (var item in result.Data)
            {
                incomes.Add((double)item.Incomes);
                expenses.Add((double)item.Expenses);
                Labels.Add(GetMonthString(item.Month));
            }

            Series =
                [
                    new ChartSeries{Name="Depósitos", Data=incomes.ToArray()},
                    new ChartSeries{Name="Despesas", Data=expenses.ToArray()}
                ];

            Options.YAxisTicks = 1000;
            Options.LineStrokeWidth = 5;
            Options.ChartPalette = [Colors.Green.Accent3, Colors.Red.Default];
        }

        #endregion

        #region Private

        private string GetMonthString(int month) => new DateTime(2000, month, 1).ToString("MMM");

        #endregion
    }
}
