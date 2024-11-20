using Dima.Core.Models.Orders;
using FluentAssertions;

namespace Dima.CoreTestes.Tests.Models.Orders
{
    [Trait("Category", "Models")]
    public class VoucherTestes
    {
        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarValoresDefaultCorretamente()
        {
            Voucher voucher = new();

            voucher.VourcherCode.Should().BeEmpty();
            voucher.Title.Should().BeEmpty();
            voucher.Description.Should().BeEmpty();
        }
    }
}
