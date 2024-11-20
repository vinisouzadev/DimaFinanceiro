using Dima.Core.Requests.Order;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Order
{
    [Trait("Category", "Requests")]
    public class GetOrderByCodeRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            GetOrderByCodeRequest request = new();

            request.Code.Should().BeEmpty();
        }
    }
}
