using Dima.Core.Responses.Stripe;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Responses.Stripe
{
    [Trait("Category", "Responses")]
    public class StripeTransactionResponseTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            StripeTransactionResponse stripeTransactionResponse = new();

            stripeTransactionResponse.Id.Should().BeEmpty();
            stripeTransactionResponse.Email.Should().BeEmpty();
            stripeTransactionResponse.Status.Should().BeEmpty();
        }
    }
}
