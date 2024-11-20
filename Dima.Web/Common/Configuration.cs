using MudBlazor.Utilities;
using MudBlazor;

namespace Dima.Web.Common
{
    public static class Configuration
    {
        public const string HttpClientName = "dima";

        public static bool IsDarkMode = true;

        public static string BackendUrl { get; set; }

        public static string StripePublicKey { get; set; }

        public static MudTheme Theme = new()
        {
            Typography = new Typography()
            {
                Default = new Default()
                {
                    FontFamily = ["Raleway", "sans-serif"]
                },
                Button = new Button()
                {
                    FontWeight = 700
                }

            },

            PaletteLight = new PaletteLight()
            {
                Primary = "#1EFA2D",
                Secondary = Colors.LightGreen.Darken3,
                Background = Colors.Gray.Lighten5,
                AppbarBackground = new MudColor("#1EFA2D"),
                AppbarText = Colors.Shades.Black,
                DrawerText = new MudColor("#ffffff"),
                DrawerBackground = new MudColor("#005905"),
                PrimaryContrastText = Colors.Shades.Black,
                Tertiary = "#1EFA2D"

            },

            PaletteDark = new PaletteDark()
            {
                Primary = Colors.LightGreen.Accent4,
                Background = new MudColor("#353535"),
                AppbarBackground = Colors.LightGreen.Accent3,
                AppbarText = Colors.Shades.Black,
                PrimaryContrastText = Colors.Shades.Black,
                Tertiary = Colors.LightGreen.Accent4

            }
        };

        
    }
}
