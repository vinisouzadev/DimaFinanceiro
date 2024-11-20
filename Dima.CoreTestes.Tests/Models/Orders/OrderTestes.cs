using Bogus;
using Dima.Core.Enums;
using Dima.Core.Models.Orders;
using FluentAssertions;
using Microsoft.VisualBasic;

namespace Dima.CoreTestes.Tests.Models.Orders
{
    [Trait("Category", "Models")]
    public class OrderTestes
    {
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        public void Construtor_DadoInstanciaSemValores_EntaoDeveSetarDefaultNasPropriedadesCorretamente()
        {
            int expectedOrderCodeLenght = 8;
            EPaymentGateway expectedGateway= EPaymentGateway.Stripe;
            EOrderStatus expectedOrderStatus = EOrderStatus.WaitingPayment;
            string expectedUserId = string.Empty;

            DateTime dateBeforeOrderInstance = DateTime.UtcNow.AddSeconds(-1);
            Order order = new();
            DateTime dateAfterOrderInstance = DateTime.UtcNow.AddSeconds(1);

            

            order.OrderCode.Length.Should().Be(expectedOrderCodeLenght);
            order.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
            order.CreatedAt.Should().BeBefore(dateAfterOrderInstance);
            order.CreatedAt.Should().BeAfter(dateBeforeOrderInstance);
            order.UpdatedAt.Kind.Should().Be(DateTimeKind.Utc);
            order.UpdatedAt.Should().BeBefore(dateAfterOrderInstance);
            order.UpdatedAt.Should().BeAfter(dateBeforeOrderInstance);
            order.Gateway.Should().Be(expectedGateway);
            order.Status.Should().Be(expectedOrderStatus);
            order.UserId.Should().Be(expectedUserId);

        }

        [Fact]
        public void Total_DadoUmProdutoComPrecoDefinidoEVoucherNulo_EntaoDeveRetornarOTotalDoPedidoCorretamente()
        {
            Product product = new();
            product.Price = _faker.Random.Decimal(0.5m);
            decimal expectedTotal = product.Price;

            Order order = new()
            {
                Product = product
            };

            order.Total.Should().Be(expectedTotal);
        }

        [Fact]
        public void Total_DadoUmProdutoComPrecoEVoucherDefinido_EntaoDeveRetornarOTotalDoPedidoCorretamente()
        {
            Product product = new();
            product.Price = _faker.Random.Decimal(10m);
            Voucher voucher = new();
            voucher.Amount = _faker.Random.Decimal(0.5m,9m);
            decimal expectedTotal = product.Price - voucher.Amount;

            Order order = new();
            order.Product = product;
            order.Voucher = voucher;

            order.Total.Should().Be(expectedTotal);
        }

        [Fact]
        public void Total_DadoUmProdutoNuloAoCalcularOTotal_EntaoDeveRetornarUmaException()
        {
            Product product = new();

            Order order = new();

            var totalWithException = () => order.Total;

            totalWithException.Should().ThrowExactly<System.NullReferenceException>();
        }
    }
}
