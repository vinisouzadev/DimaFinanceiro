namespace Dima.API.Common
{
    public static class ApiConfiguration
    {
        public const string CorsPolicyName = "BlazorWebAssembly";

        public static string StripeApIKey { get; set; } = string.Empty;
    }
}
