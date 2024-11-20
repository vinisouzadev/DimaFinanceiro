using Dima.Core.Requests.Transactions;
using FluentAssertions;
using Dima.Core.Enums;

namespace Dima.CoreTestes.Tests.Requests.Transactions
{
    [Trait("Category", "Requests")]
    public class CreateTransactionRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            CreateTransactionRequest request = new();

            request.Title.Should().BeEmpty();
            request.Type.Should().Be(ETransactionType.Withdraw);
        }
    }
}
