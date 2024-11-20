﻿using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports
{
    public partial class ExpensesByCategoryChartComponent : ComponentBase
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
            await GetExpensesByCategoryAsync();
        }

        #endregion

        #region Private

        private async Task GetExpensesByCategoryAsync()
        {
            var request = new GetExpensesByCategoryRequest();
            var result = await Handler.GetExpensesByCategoryAsync(request);
            if (!result.IsSuccess || result.Data is null)
            {
                Snackbar.Add(result.Message, Severity.Error);
                return;
            }

            foreach(var item in result.Data)
            {
                Labels.Add($"{item.Category} ({item.Expenses:C})");
                Data.Add(-(double)item.Expenses);
            }
        }

        #endregion
    }
}