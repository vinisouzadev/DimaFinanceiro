using Dima.Core.Common;
using FluentAssertions;

namespace Dima.CoreTestes.Tests
{
    [Trait("Category", "Configuration")]
    public class ConfigurationTestes
    {
        [Fact]
        public void Propriedades_QuandoNaoHouverValorSetado_EntaoDeveRetornarValoresDefault()
        {   
            Configuration.ConnectionString.Should().BeEmpty();
            Configuration.BackendUrl.Should().BeEmpty();
            Configuration.FrontendUrl.Should().BeEmpty();
        }

        [Fact]
        public void Constantes_QuandoComparadoComValoresDeDefaultCorretos_EntaoDeveRetornarTrue()
        {
            int expectedDefaultPageSize = 25;
            int expectedDefaultStatusCode = 200;

            Configuration.DefaultPageSize.Should().Be(expectedDefaultPageSize);
            Configuration.DefaultStatusCode.Should().Be(expectedDefaultStatusCode);
        }
    }
}
