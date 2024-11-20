using Dima.Core.Enums;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Enums
{
    [Trait("Category", "Enum")]
    public class EPaymentGatewayTestes
    {
        [Fact]
        public void ValoresEnum_DadoInputCorretoAoEnum_EntaoDeveAssociarAoValorCorreto()
        {
            int inputStripe = 0;
            int inputPayPal = 1;
            int inputPagarMe = 2;

            EPaymentGateway stripe = EPaymentGateway.Stripe;
            EPaymentGateway payPal = EPaymentGateway.PayPal;
            EPaymentGateway pagarMe = EPaymentGateway.PagarMe;

            stripe.Should().Be((EPaymentGateway)inputStripe);
            payPal.Should().Be((EPaymentGateway)inputPayPal);
            pagarMe.Should().Be((EPaymentGateway)inputPagarMe);
        }
    }
}
