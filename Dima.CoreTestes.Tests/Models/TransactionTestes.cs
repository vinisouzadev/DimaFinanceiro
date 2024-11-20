using Dima.Core.Models;
using FluentAssertions;
using Dima.Core.Enums;

namespace Dima.CoreTestes.Tests.Models
{
    [Trait("Category", "Models")]
    public class TransactionTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            Transaction transaction = new();

            transaction.Title.Should().BeEmpty();
            transaction.Type.Should().Be(ETransactionType.Withdraw);
        }
    }
}
