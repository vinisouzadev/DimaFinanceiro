using Dima.Core.Enums;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Enums
{
    [Trait("Category", "Enum")]
    public class ETransactionTypeTestes
    {
        [Fact]
        public void ValoresEnum_DadoInputCorretoAoEnum_EntaoDeveAssociarAoValorCorreto()
        {
            int inputDeposit = 1;
            int inputWithdraw = 2;

            ETransactionType deposit = ETransactionType.Deposit;
            ETransactionType withdraw = ETransactionType.Withdraw;

            deposit.Should().Be((ETransactionType)inputDeposit);
            withdraw.Should().Be((ETransactionType)inputWithdraw);
        }
    }
}
