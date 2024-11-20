using Dima.API.Handlers;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transactions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions
{
    public partial class CreateTransactionPageCode : ComponentBase
    {
        #region Dependências

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public ITransactionHandler TransactionHandler { get; set; } = null!;

        [Inject]
        public ICategoryHandler CategoryHandler { get; set; } = null!;

        #endregion

        #region Propriedades

        public CreateTransactionRequest InputModel { get; set; } = new();

        public GetAllCategoryRequest GetAllCategoryRequest { get; set; } = new();

        public List<Category> Categories { get; set; } = [];

        public bool IsBusy {get; set; } = false;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;

            try
            {
                var result = await CategoryHandler.GetAllAsync(GetAllCategoryRequest);
                if (result.IsSuccess)
                {
                    Categories = result.Data ?? [];
                    InputModel.CategoryId = Categories.FirstOrDefault()?.Id ?? 0;
                }
                else
                    Snackbar.Add(result.Message, Severity.Error);
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

        #region Methods 

        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;

            try
            {
                var result = await TransactionHandler.CreateAsync(InputModel);
                if (result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Success);
                    StateHasChanged();
                    NavigationManager.NavigateTo("/lancamentos/historico");
                }
                else
                    Snackbar.Add(result.Message, Severity.Error);
            
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
