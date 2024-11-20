using Dima.Core.Requests.Transactions;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Transactions
{
    [Trait("Category", "Requests")]
    public class UpdateTransactionRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            UpdateTransactionRequest request = new();

            request.Title.Should().BeEmpty();
        }
    }
}
