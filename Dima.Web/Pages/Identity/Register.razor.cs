using Dima.Core.Handlers;
using Dima.Core.Requests.Account;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Dima.Web.Pages.Identity
{
    public partial class RegisterPageCode : ComponentBase
    {
        #region Dependencias

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IAccountHandler AccountHandler { get; set; } = null!;

        [Inject]
        public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Properties

        public bool IsBusy = false;

        public RegisterRequest InputModel { get; set; } = new();

        #endregion

        #region Override

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is { IsAuthenticated: true })
            {
                NavigationManager.NavigateTo("/");
            }
               
        }

        #endregion

        #region Methods

        public async Task OnValidSubtmitAsync()
        {
            IsBusy = true;

            try
            {
                
                var result = await AccountHandler.RegisterAsync(InputModel);

                if (result.IsSuccess)
                {
                    Snackbar.Add(result.Message, Severity.Success);
                    NavigationManager.NavigateTo("/login");
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

        #endregion
    }
}
