using Dima.Core.Requests.Order;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Order
{
    [Trait("Category", "Requests")]
    public class PayOrderRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            PayOrderRequest request = new();

            request.OrderNumber.Should().BeEmpty();
            request.ExternalReference.Should().BeEmpty();
        }
    }
}
