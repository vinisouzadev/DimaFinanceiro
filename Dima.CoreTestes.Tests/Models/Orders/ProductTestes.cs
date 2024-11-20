using Dima.Core.Models.Orders;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Models.Orders
{
    [Trait("Category", "Models")]
    public class ProductTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            Product product = new();

            product.Title.Should().BeEmpty();
            product.Description.Should().BeEmpty();
            product.Slug.Should().BeEmpty();
        }
    }
}
