using Dima.Core.Requests.Order;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Order
{
    [Trait("Category", "Requests")]
    public class GetVoucherByCodeRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            GetVoucherByCodeRequest request = new();

            request.Code.Should().BeEmpty();
        }
    }
}
