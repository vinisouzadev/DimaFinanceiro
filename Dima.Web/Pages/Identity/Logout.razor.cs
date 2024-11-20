using Dima.Core.Handlers;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;

namespace Dima.Web.Pages.Identity
{
    public partial class LogoutPageCode : ComponentBase
    {
        #region Dependencias

        [Inject]
        public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IAccountHandler AccountHandler { get; set; } = null!;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            if(await AuthenticationStateProvider.CheckAutheticatedAsync())
            {
                await AccountHandler.LogoutAsync();
                await AuthenticationStateProvider.GetAuthenticationStateAsync();
                AuthenticationStateProvider.NotifyAuthenticationStateChanged();
            }

            await base.OnInitializedAsync();
        }

        #endregion
    }
}
