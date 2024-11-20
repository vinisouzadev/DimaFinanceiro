using Dima.API.Handlers;
using Dima.Core.Requests.Categories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Categories
{
    public partial class EditCategoryPageCode : ComponentBase
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

        [Parameter]
        public string Id { get; set; } = string.Empty;

        public UpdateCategoryRequest InputModel { get; set; } = new();

        public bool IsBusyGet { get; set; } = false;

        public bool IsBusyUpdate { get; set; } = false;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            IsBusyGet = true;

            try
            {
                if (!long.TryParse(Id, out var id))
                    Snackbar.Add("Erro ao converter tipagem de Id", Severity.Error);
                else
                {
                    var request = new GetByIdCategoryRequest { Id = id };
                    var result = await Handler.GetByIdAsync(request);
                    if (result is { IsSuccess: true, Data: not null })
                    {
                        InputModel = new()
                        {
                            Id = result.Data.Id,
                            Title = result.Data.Title,
                            Description = result.Data.Description
                        };
                    }
                    else
                        Snackbar.Add(result.Message, Severity.Error);
                        
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally
            {
                IsBusyGet = false;
            }
                
        }

        #endregion

        #region Methods

        public async Task OnValidSubmitAsync()
        {
            IsBusyUpdate = true;

            try
            {
                var result = await Handler.UpdateAsync(InputModel);
                if (result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Success);
                    NavigationManager.NavigateTo("/categorias");
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
                IsBusyUpdate = false;
            }
        }

        #endregion

    }
}
