using Dima.API.Handlers;
using Dima.Core.Requests.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Categories
{
    public partial class CreateCategoryPageCode : ComponentBase
    {
        #region Dependencias

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public ICategoryHandler Handler { get; set; } = null!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        #endregion

        #region Propriedades

        public bool IsBusy = false;

        public CreateCategoryRequest InputModel { get; set; } = new();

        #endregion

        #region Metódos

        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;
            try
            {
                var result = await Handler.CreateAsync(InputModel);
                if (result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Success);
                    NavigationManager.NavigateTo("/categorias");
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
    }
}
