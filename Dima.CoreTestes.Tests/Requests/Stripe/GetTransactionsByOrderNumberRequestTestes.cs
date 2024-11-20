using Dima.Core.Requests.Stripe;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Stripe
{
    [Trait("Category", "Requests")]
    public class GetTransactionsByOrderNumberRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            GetTransactionsByOrderNumberRequest request = new();

            request.OrderNumber.Should().BeEmpty();
        }
    }
}
