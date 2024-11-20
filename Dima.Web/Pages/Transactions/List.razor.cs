using Dima.Core.Handlers;
using Dima.Core.Requests.Transactions;
using Microsoft.AspNetCore.Components;
using Dima.Core.Models;
using MudBlazor;
using Dima.Core.Common.Extensions;

namespace Dima.Web.Pages.Transactions
{
    public partial class ListTransactionPageCode : ComponentBase
    {
        #region Dependências

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IDialogService DialogService { get; set; } = null!;

        [Inject]
        public ITransactionHandler Handler { get; set; } = null!;

        #endregion

        #region Propriedades

        public GetByPeriodTransactionRequest InputModel { get; set; } = new();

        public bool IsBusy { get; set; } = false;

        public List<Transaction> Transactions { get; set; } = [];

        public string SearchText { get; set; } = string.Empty;

        public int CurrentYear { get; set; } = DateTime.UtcNow.Year;

        public int CurrentMonth { get; set; } = DateTime.UtcNow.Month;

        public int[] Years { get; set; } =
            [
                DateTime.UtcNow.Year,
                DateTime.UtcNow.AddYears(-1).Year,
                DateTime.UtcNow.AddYears(-2).Year,
                DateTime.UtcNow.AddYears(-3).Year

            ];

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync() => await GetTransaction();

        #endregion

        #region Public

        public Func<Transaction, bool> Search => transaction =>
        {
            if (string.IsNullOrEmpty(SearchText))
                return true;
            if (transaction.Id.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || transaction.Title != null && transaction.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        public async Task OnDeleteButtonClickedAsync(long id)
        {
            var result = await DialogService.ShowMessageBox("ATENÇÃO",
                "A transação será deletada permanentemente se optar por sua exclusão. Deseja continuar?", yesText:"Excluir", cancelText :"Cancelar");
            if (result == true)
                await OnDeleteAsync(id);

        }
        
        public async Task FilterDate()
        {
            await GetTransaction();
            StateHasChanged();
        }
        
        #endregion

        #region Private

        private async Task GetTransaction()
        {
            IsBusy = true;

            try
            {
                var request = new GetByPeriodTransactionRequest()
                {
                    StartDate = DateTime.UtcNow.GetFirstDay(CurrentYear, CurrentMonth),
                    EndDate = DateTime.UtcNow.GetLastDay(CurrentYear, CurrentMonth),
                    PageNumber = 1,
                    PageSize = 1000
                };

                var result = await Handler.GetByPeriodAsync(request);
                if (result.IsSuccess)
                {
                    Transactions = result.Data ?? [];
                    StateHasChanged();
                }
                else
                    Snackbar.Add(result.Message, Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnDeleteAsync(long id)
        {
            IsBusy = true;
            try
            {
                DeleteTransactionRequest request = new() { Id = id };
                var result = await Handler.DeleteAsync(request);
                if (result.IsSuccess)
                {
                    Snackbar.Add(result.Message,Severity.Success);
                    Transactions.RemoveAll(t => t.Id == id);
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
