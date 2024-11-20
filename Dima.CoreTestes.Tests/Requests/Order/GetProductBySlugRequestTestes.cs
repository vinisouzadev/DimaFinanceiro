using Dima.Core.Requests.Order;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Order
{
    [Trait("Category", "Requests")]
    public class GetProductBySlugRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            GetProductBySlugRequest request = new();

            request.Slug.Should().BeEmpty();
        }
    }
}
