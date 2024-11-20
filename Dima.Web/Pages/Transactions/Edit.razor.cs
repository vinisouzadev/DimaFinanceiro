using Dima.API.Handlers;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transactions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions
{
    public partial class EditTransactionPageCode : ComponentBase
    {
        #region Dependencias

        [Inject]
        public ITransactionHandler TransactionHandler { get; set; } = null!;

        [Inject]
        public ICategoryHandler CategoryHandler { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        #endregion

        #region Propriedades

        public UpdateTransactionRequest InputModel { get; set; } = new();

        public bool IsBusy { get; set; } = false;

        [Parameter]
        public string Id { get; set; } = string.Empty;

        public List<Category> Categories { get; set; } = [];
        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;

            await GetByIdTransactionAsync();
            await GetAllCategoryAsync();

            IsBusy = false;
        }

        #endregion

        #region

        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;

            try
            {
                var result = await TransactionHandler.UpdateAsync(InputModel);
                if (result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Success);
                    NavigationManager.NavigateTo("/lancamentos/historico");
                }
                else
                {
                    Snackbar.Add(result.Message, Severity.Error);
                }
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
        #endregion

        #region Private

        private async Task GetAllCategoryAsync()
        {
            var request = new GetAllCategoryRequest();

            try
            {
                var result = await CategoryHandler.GetAllAsync(request);
                if (result.IsSuccess)
                {
                    Categories = result.Data ?? [];
                }
                else
                {
                    Snackbar.Add(result.Message, Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }

        }

        private async Task GetByIdTransactionAsync()
        {
            var request = new GetByIdTransactionRequest() { Id = long.Parse(Id) };

            try
            {
                var result = await TransactionHandler.GetByIdAsync(request);
                if (result.IsSuccess && result.Data is not null)
                {
                    InputModel = new UpdateTransactionRequest
                    {
                        CategoryId = result.Data.CategoryId,
                        Title = result.Data.Title,
                        PaidOrReceivedAt = result.Data.PaidOrReceivedAt,
                        Amount = result.Data.Amount,
                        Type = result.Data.Type,
                        Id = result.Data.Id
                    };
                }
                else
                {
                    Snackbar.Add(result.Message, Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }

        }

        #endregion
    }
}

