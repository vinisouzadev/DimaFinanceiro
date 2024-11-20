using Dima.Core.Enums;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Enums
{
    [Trait("Category", "Enum")]
    public class EOrderStatusTestes
    {
        [Fact]
        public void ValoresEnum_DadoInputCorretoAoEnum_DeveAssociarAoValorCorreto()
        {
            int inputWaitingPayment = 0;
            int inputCanceled = 1;
            int inputPaid = 2;
            int inputRefunded = 3;

            EOrderStatus waitingPayment = EOrderStatus.WaitingPayment;
            EOrderStatus canceled = EOrderStatus.Canceled;
            EOrderStatus paid = EOrderStatus.Paid;
            EOrderStatus refunded =EOrderStatus.Refunded;

            waitingPayment.Should().Be((EOrderStatus)inputWaitingPayment);
            canceled.Should().Be((EOrderStatus)inputCanceled);
            paid.Should().Be((EOrderStatus)inputPaid);
            refunded.Should().Be((EOrderStatus)inputRefunded);
        }
    }
}
