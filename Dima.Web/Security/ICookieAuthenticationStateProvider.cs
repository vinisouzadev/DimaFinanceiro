using Microsoft.AspNetCore.Components.Authorization;

namespace Dima.Web.Security
{
    public interface ICookieAuthenticationStateProvider
    {
        Task<bool> CheckAutheticatedAsync();

        Task<AuthenticationState> GetAuthenticationStateAsync();

        void NotifyAuthenticationStateChanged();
    }
}
