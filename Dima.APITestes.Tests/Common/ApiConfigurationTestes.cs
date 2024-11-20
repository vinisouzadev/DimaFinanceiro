using Dima.API.Common;
using FluentAssertions;

namespace Dima.APITestes.Tests.Common
{
    [Trait("Category", "Configuration")]
    public class ApiConfigurationTestes
    {
        [Fact]
        public void Constantes_QuandoComparadoComValoresCorretos_EntaoDeveRetornarTrue()
        {
            string expectedCorsPolicyName = "BlazorWebAssembly";

            ApiConfiguration.CorsPolicyName.Should().Be(expectedCorsPolicyName);
        }

        [Fact]
        public void Propriedades_QuandoComparadoComValoresDefaultCorreto_EntaoDeveRetornarTrue()
        {
            ApiConfiguration.StripeApIKey.Should().BeEmpty();
        }
    }
}
