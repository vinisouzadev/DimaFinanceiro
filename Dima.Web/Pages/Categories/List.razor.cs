using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Categories
{
    public partial class ListCategoryPageCode : ComponentBase
    {
        #region Dependencias

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IDialogService DialogService { get; set; } = null!;

        [Inject]
        public ICategoryHandler Handler { get; set; } = null!;

        #endregion

        #region Propriedades

        public List<Category> Categories { get; set; } = [];

        public GetAllCategoryRequest Request { get; set; } = new();

        public bool IsBusy = false;

        public string SearchText { get; set; } = string.Empty;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;

            try
            {
                var result = await Handler.GetAllAsync(Request);
                if (result.IsSuccess)
                    Categories = result.Data ?? [];
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

        #endregion

        #region Methods

        public async void OnDeleteButtonClickedAsync(long id, string title)
        {
            var result = await DialogService.ShowMessageBox(
                "ATENÇÃO",
                $"A categoria {title} será deletada. Isso é irreversível! Deseja realmente continuar?",
                yesText: "Excluir",
                cancelText: "Cancelar");

            if (result is true)
            {
                await OnDeleteAsync(id);
                StateHasChanged();
            }
                
        }

        public async Task OnDeleteAsync(long id) 
        {

            var request = new DeleteCategoryRequest { Id = id };
            try
            {
                var result = await Handler.DeleteAsync(request);

                if (result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Success);
                    Categories.RemoveAll(x => x.Id == id);
                }
                else
                    Snackbar.Add(result.Message, Severity.Error);
            }
            catch(Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

        public Func<Category, bool> SearchFilter => category =>
        {
            if (string.IsNullOrEmpty(SearchText))
                return true;

            if (category.Id.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return true;

            if (category.Title is not null && category.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return true;

            if (category.Description is not null && category.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        #endregion

    }
}
