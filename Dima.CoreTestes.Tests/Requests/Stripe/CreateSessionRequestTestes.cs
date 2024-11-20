using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Stripe;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Requests.Stripe
{
    [Trait("Category", "Requests")]
    public class CreateSessionRequestTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            CreateSessionRequest request = new();

            request.OrderNumber.Should().BeEmpty();
            request.ProductTitle.Should().BeEmpty();
            request.ProductDescription.Should().BeEmpty();
        }
    }
}
