using Bogus;
using Dima.API.Data;
using Dima.API.Handlers;
using Dima.Core.Enums;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using Dima.Core.Responses.Stripe;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dima.APITestes.Tests.Handlers
{
    [Trait("Category", "Handlers")]
    public class OrderHandlerTestes
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly AppDbContext _appDbContext;

        private readonly IStripeHandler _mockStripeHandler;

        private readonly OrderHandler _orderHandler;

        public OrderHandlerTestes()
        {
            DbContextOptionsBuilder<AppDbContext> dbContextOptions = new();
            dbContextOptions.UseInMemoryDatabase("orderHandlerContext");
            _appDbContext = new(dbContextOptions.Options);

            _mockStripeHandler = Substitute.For<IStripeHandler>();

            OrderHandler orderHandler = new(_appDbContext, _mockStripeHandler);
            _orderHandler = orderHandler;
        }


        #region CancelOrderAsync

        [Fact]
        public async Task CancelOrderAsycn_DadoUmIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyUserId = _faker.Person.UserName;
            Order anyOrder = new()
            {
                UserId = anyUserId
            };

            _appDbContext.Orders.Add(anyOrder);
            _appDbContext.SaveChanges();

            CancelOrderRequest request = new()
            {
                Id = _faker.Random.Int(-10, -1),
                UserId = anyUserId
            };

            var result = await _orderHandler.CancelOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado");
        }

        [Fact]
        public async Task CancelOrderAsync_DadoUmUserIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyUserId = _faker.Person.UserName;
            Order anyOrder = new()
            {
                UserId = anyUserId
            };

            _appDbContext.Orders.Add(anyOrder);
            _appDbContext.SaveChanges();

            CancelOrderRequest request = new()
            {
                Id = anyOrder.Id,
                UserId = _faker.Person.FirstName
            };

            var result = await _orderHandler.CancelOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado");
        }

        [Theory]
        [InlineData(EOrderStatus.Canceled)]
        [InlineData(EOrderStatus.Paid)]
        [InlineData(EOrderStatus.Refunded)]
        public async Task CancelOrderAsync_DadoUmPedidoComStatusDiferenteDeAguardandoPagamento_EntaoDeveRetornarRespostaDeFalha(EOrderStatus orderStatus)
        {

            Product anyProduct = new()
            {
                Title = _faker.Person.FirstName
            };
            _appDbContext.Products.Add(anyProduct);
            _appDbContext.SaveChanges();


            string anyUserId = _faker.Person.FirstName;
            Order anyOrder = new()
            {
                UserId = anyUserId,
                Status = orderStatus,
                Product = anyProduct
            };

            _appDbContext.Orders.Add(anyOrder);
            await _appDbContext.SaveChangesAsync();

            CancelOrderRequest request = new()
            {
                Id = anyOrder.Id,
                UserId = anyUserId
            };

            var teste = await _appDbContext.Orders
                    .Include(x => x.Voucher)
                    .Include(x => x.Product)
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            var result = await _orderHandler.CancelOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Este pedido não pode ser cancelado");
        }

        [Fact]
        public async Task CancelOrderAsync_DadoCancelamentoDoPedido_EntaoDeveSalvarCorretamenteAsMudancasNoBanco()
        {
            string anyProductTitle = _faker.Person.FirstName;
            Product product = new()
            {
                Title = anyProductTitle
            };

            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();

            string anyUserId = _faker.Person.UserName;
            Order order = new()
            {
                UserId = anyUserId,
                Product = product,
                Status = EOrderStatus.WaitingPayment
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            CancelOrderRequest request = new()
            {
                UserId = order.UserId,
                Id = order.Id
            };

            DateTime dateTimeBeforeHandler = DateTime.UtcNow;
            var result = await _orderHandler.CancelOrderAsync(request);
            DateTime dateTimeAfterHandler = DateTime.UtcNow;

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Product.Should().NotBeNull();
            result.Data.Status.Should().Be(EOrderStatus.Canceled);
            result.Data.UpdatedAt.Should().BeBefore(dateTimeAfterHandler);
            result.Data.UpdatedAt.Should().BeAfter(dateTimeBeforeHandler);

        }

        #endregion

        #region GetAllOrderAsync

        [Fact]
        public async Task GetAllOrderAsync_DadoUmaListaDePedidosDeUmUsuario_EntaoDeveRetornarTodosOsPedidosDesteUsuario()
        {
            string anyTitleForProduct = _faker.Person.UserName;
            Product product = new()
            {
                Title = anyTitleForProduct
            };

            _appDbContext.Add(product);
            _appDbContext.SaveChanges();

            string firstUserId = _faker.Person.UserName;
            Order firstOrder = new()
            {
                UserId = firstUserId,
                Product = product
            };

            Order secondOrder = new()
            {
                UserId = firstUserId,
                Product = product
            };

            string secondUserId = _faker.Person.UserName;
            Order thirdOrder = new()
            {
                UserId = secondUserId,
                Product = product
            };


            _appDbContext.Add(firstOrder);
            _appDbContext.Add(secondOrder);
            _appDbContext.Add(thirdOrder);
            _appDbContext.SaveChanges();

            GetAllOrdersRequest request = new()
            {
                UserId = firstUserId
            };

            var result = await _orderHandler.GetAllOrdersAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            foreach (var item in result.Data!)
            {
                item.Product.Should().NotBeNull();
            }
            result.Data!.All(x => x.UserId == firstUserId).Should().BeTrue();
        }


        #endregion

        #region GetOrderByCodeAsync

        [Fact]
        public async Task GetOrderByCodeAsync_DadoUmUserIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {

            string incorrectlyUserId = _faker.Person.FullName;

            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };
            string anyUserId = _faker.Person.UserName;
            Order order = new()
            {
                UserId = anyUserId,
                Product = product
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            GetOrderByCodeRequest request = new()
            {
                UserId = incorrectlyUserId,
                Code = order.OrderCode
            };

            var result = await _orderHandler.GetOrderByCodeAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado");
        }

        [Fact]
        public async Task GetOrderByCodeAsync_DadoUmOrderCodeIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };
            string incorrectlyOrderCode = _faker.Lorem.Paragraph();

            string anyUserId = _faker.Person.LastName;
            Order order = new()
            {
                UserId = anyUserId,
                Product = product
            };
            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            GetOrderByCodeRequest request = new()
            {
                UserId = anyUserId,
                Code = incorrectlyOrderCode
            };

            var result = await _orderHandler.GetOrderByCodeAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado");

        }

        [Fact]
        public async Task GetOrderByCodeAsync_DadoUmUserIdEOrderCodeCorreto_EntaoDeveRetornarOPedidoCorretamente()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string correctlyUserId = _faker.Person.FirstName;
            Order order = new()
            {
                UserId = correctlyUserId,
                Product = product
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            GetOrderByCodeRequest request = new()
            {
                UserId = correctlyUserId,
                Code = order.OrderCode
            };

            var result = await _orderHandler.GetOrderByCodeAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.UserId.Should().Be(correctlyUserId);
            result.Data.OrderCode.Should().Be(order.OrderCode);
        }

        #endregion

        #region PayOrderAsync

        [Fact]
        public async Task PayOrderAsync_DadoUmUserIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();

            string incorrectlyUserId = _faker.Random.Utf16String(10);
            Order order = new()
            {
                UserId = _faker.Person.FirstName,
                Product = product

            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                OrderNumber = order.OrderCode,
                UserId = incorrectlyUserId
            };

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado");
        }

        [Fact]
        public async Task PayOrderAsync_DadoUmOrderCodeIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();

            string incorrectlyOrderCode = _faker.Random.Utf16String(10);
            string correctlyUserId = _faker.Person.FirstName;
            Order order = new()
            {
                UserId = correctlyUserId,
                Product = product

            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                OrderNumber = incorrectlyOrderCode,
                UserId = correctlyUserId
            };

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado");
        }

        [Fact]
        public async Task PayOrderAsync_DadoUmPedidoComStatusCanceled_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.LastName;
            Order order = new()
            {
                UserId = anyUserId,
                Product = product,
                Status = EOrderStatus.Canceled
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                OrderNumber = order.OrderCode,
                UserId = order.UserId
            };

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Data!.OrderCode.Should().Be(order.OrderCode);
            result.Message.Should().Be("Este pedido já foi cancelado e não pode ser pago");
        }

        [Fact]
        public async Task PayOrderAsync_DadoUmPedidoComStatusPaid_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.FirstName;
            Order order = new()
            {
                UserId = anyUserId,
                Product = product,
                Status = EOrderStatus.Paid
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = order.UserId,
                OrderNumber = order.OrderCode
            };

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Data!.OrderCode.Should().Be(order.OrderCode);
            result.Message.Should().Be("Este pedido já foi pago");
        }

        [Fact]
        public async Task PayOrderAsync_DadoUmPedidoComStatusRefunded_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.FirstName;
            Order order = new()
            {
                UserId = anyUserId,
                Product = product,
                Status = EOrderStatus.Refunded
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = order.UserId,
                OrderNumber = order.OrderCode
            };

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Data!.OrderCode.Should().Be(order.OrderCode);
            result.Message.Should().Be("Este pedido já foi reembolsado e não pode ser pago novamente.");
        }

        [Fact]
        public async Task PayOrderAsync_DadoIsSuccessFalsoComoRespostaDeGetTransactionByOrderAsync_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.UserName;
            Order order = new()
            {
                Product = product,
                UserId = anyUserId,
                Status = EOrderStatus.WaitingPayment
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = order.UserId,
                OrderNumber = order.OrderCode
            };

            Response<List<StripeTransactionResponse>> mockStripeHandlerResponse = new([], 500, string.Empty);
            _mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Returns(mockStripeHandlerResponse);

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Não foi possível encontrar este pagamento");
        }

        [Fact]
        public async Task PayOrderAsync_DadoPropriedadeDataComoNullVindoDeGetTransactionByOrderNumberAsync_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.UserName;
            Order order = new()
            {
                Product = product,
                UserId = anyUserId,
                Status = EOrderStatus.WaitingPayment
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = order.UserId,
                OrderNumber = order.OrderCode
            };

            Response<List<StripeTransactionResponse>> mockStripeHandlerResponse = new(null, 200, string.Empty);
            _mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Returns(mockStripeHandlerResponse);

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível encontrar este pagamento");
        }

        [Fact]
        public async Task PayOrderAsycn_DadoTransacaoComStatusRefundedVindoDeGetTransactionByOrderNumberAsync_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.UserName;
            Order order = new()
            {
                Product = product,
                UserId = anyUserId,
                Status = EOrderStatus.WaitingPayment
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = order.UserId,
                OrderNumber = order.OrderCode
            };

            List<StripeTransactionResponse> stripeTransactionResponses =
                [
                    new StripeTransactionResponse(){
                        Id = _faker.Random.Utf16String(),
                        Refunded = true
                    },
                    new StripeTransactionResponse(){
                        Id = _faker.Random.Utf16String(),
                        Refunded = false
                    }
                ];

            Response<List<StripeTransactionResponse>> mockStripeHandlerResponse = new(stripeTransactionResponses, 200, string.Empty);
            _mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Returns(mockStripeHandlerResponse);

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Este pedido já foi estornado");
        }

        [Fact]
        public async Task PayOrderAsync_DadoTransacaoSemStatusPaidVindoDeGetTransactionByOrderNumberAsync_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.UserName;
            Order order = new()
            {
                Product = product,
                UserId = anyUserId,
                Status = EOrderStatus.WaitingPayment
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = order.UserId,
                OrderNumber = order.OrderCode
            };

            List<StripeTransactionResponse> stripeTransactionResponses =
                [
                    new StripeTransactionResponse(){
                        Id = _faker.Random.Utf16String(),
                        Refunded = false,
                        Paid = false
                    },
                    new StripeTransactionResponse(){
                        Id = _faker.Random.Utf16String(),
                        Refunded = false,
                        Paid = false
                    }
                ];

            Response<List<StripeTransactionResponse>> mockStripeHandlerResponse = new(stripeTransactionResponses, 200, string.Empty);
            _mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Returns(mockStripeHandlerResponse);

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Este pedido ainda não foi pago");
        }

        [Fact]
        public async Task PayOrderAsync_DadoExceptionDuranteGetTransactionByOrderNumberAsync_EntaoDeveRetornarRespostaDeFalha()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.UserName;
            Order order = new()
            {
                Product = product,
                UserId = anyUserId,
                Status = EOrderStatus.WaitingPayment
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = order.UserId,
                OrderNumber = order.OrderCode
            };

            Exception anyException = new("Erro simulado para teste");
            _mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Throws(anyException);

            var result = await _orderHandler.PayOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível dar baixa no seu pedido! Entre em contato com nosso suporte!");
        }

        [Fact]
        public async Task PayOrderAsync_DadoPedidoPagoComSucesso_EntaoDeveAtualizarPedidoNoBancoComSucesso()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.LastName;
            Order orderTest = new()
            {
                UserId = anyUserId,
                Product = product
            };

            _appDbContext.Orders.Add(orderTest);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = anyUserId,
                OrderNumber = orderTest.OrderCode
            };

            string expectedExternalReference = _faker.Random.Utf16String();
            List<StripeTransactionResponse> stripeTransactionResponses =
                [
                    new StripeTransactionResponse()
                    {
                        Id = expectedExternalReference,
                        Paid = true,
                        Refunded = false
                    }
                ];
            Response<List<StripeTransactionResponse>> stripeHandlerResponse = new(stripeTransactionResponses, 200, string.Empty);
            _mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Returns(stripeHandlerResponse);

            _appDbContext.ChangeTracker.Clear();
            DateTime dateTimeBeforePayOrder = DateTime.UtcNow;
            var result = await _orderHandler.PayOrderAsync(request);
            DateTime dateTimeAfterPayOrder = DateTime.UtcNow;

            var updatedOrder = await _appDbContext.Orders.SingleOrDefaultAsync(x => x.UserId == anyUserId && x.OrderCode == orderTest.OrderCode);

            updatedOrder.Should().NotBeNull();
            updatedOrder!.Product.Should().NotBeNull();
            updatedOrder.ExternalReference.Should().Be(expectedExternalReference);
            updatedOrder.Status.Should().Be(EOrderStatus.Paid);
            updatedOrder.UpdatedAt.Should().BeBefore(dateTimeAfterPayOrder);
            updatedOrder.UpdatedAt.Should().BeAfter(dateTimeBeforePayOrder);
        
        }

        [Fact]
        public async Task PayOrderAsync_DadoPedidoPagoComSucesso_EntaoDeveGerarRespostaDeSucesso()
        {
            string anyProductTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = anyProductTitle
            };

            string anyUserId = _faker.Person.LastName;
            Order order = new()
            {
                UserId = anyUserId,
                Product = product
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            PayOrderRequest request = new()
            {
                UserId = anyUserId,
                OrderNumber = order.OrderCode
            };

            string expectedExternalReference = _faker.Random.Utf16String();
            List<StripeTransactionResponse> stripeTransactionResponses =
                [
                    new StripeTransactionResponse()
                    {
                        Id = expectedExternalReference,
                        Paid = true,
                        Refunded = false
                    }
                ];
            Response<List<StripeTransactionResponse>> stripeHandlerResponse = new(stripeTransactionResponses, 200, string.Empty);
            _mockStripeHandler.GetTransactionsByOrderNumberAsync(Arg.Any<GetTransactionsByOrderNumberRequest>()).Returns(stripeHandlerResponse);

            DateTime dateTimeBeforePayOrder = DateTime.UtcNow;
            var result = await _orderHandler.PayOrderAsync(request);
            DateTime dateTimeAfterPayOrder = DateTime.UtcNow;

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Product.Should().NotBeNull();
            result.Data.ExternalReference.Should().Be(expectedExternalReference);
            result.Data.Status.Should().Be(EOrderStatus.Paid);
            result.Data.UpdatedAt.Should().BeBefore(dateTimeAfterPayOrder);
            result.Data.UpdatedAt.Should().BeAfter(dateTimeBeforePayOrder);
            
        }

        #endregion

        #region RefundOrderAsync

        [Fact]
        public async Task RefundOrderAsync_DadoIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string productTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = productTitle
            };

            string userId = _faker.Person.FullName;
            Order order = new()
            {
                UserId = userId,
                Product = product
            };

            _appDbContext.Orders.Add(order);
            await _appDbContext.SaveChangesAsync();

            long incorrectlyId = -1;
            RefundOrderRequest request = new()
            {
                Id = incorrectlyId,
                UserId = userId,
            };

            var result = await _orderHandler.RefundOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado.");
        }

        [Fact]
        public async Task RefundOrderAsync_DadoUserIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string productTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = productTitle
            };

            string userId = _faker.Person.FirstName;
            Order order = new() 
            {
                UserId = userId,
                Product = product
            };

            _appDbContext.Orders.Add(order);
            await _appDbContext.SaveChangesAsync();

            string incorrectlyUserId = _faker.Random.Utf16String();
            RefundOrderRequest request = new()
            {
                UserId = incorrectlyUserId,
                Id = order.Id
            };

            var result = await _orderHandler.RefundOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Pedido não encontrado.");
        }

        [Fact]
        public async Task RefundOrderAsync_DadoPedidoComStatusWaitintPayment_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string productTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = productTitle
            };

            string userId = _faker.Person.LastName;
            Order order = new()
            {
                UserId = userId,
                Product = product,
                Status = EOrderStatus.WaitingPayment
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            RefundOrderRequest request = new()
            {
                Id = order.Id,
                UserId = userId
            };

            var result = await _orderHandler.RefundOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(order.Id);
            result.Data.Product.Id.Should().Be(product.Id);
            result.Message.Should().Be("Este pedido ainda não foi pago e por isso não pode ser reembolsado");
        }

        [Fact]
        public async Task RefundOrderAsycn_DadoPedidoComStatusCanceled_EntaoDeveRetornarRespostaDeFalha()
        {
            string productTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = productTitle
            };

            string userId = _faker.Person.FullName;
            Order order = new()
            {
                UserId = userId,
                Product = product,
                Status = EOrderStatus.Canceled
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            RefundOrderRequest request = new()
            {
                UserId = userId,
                Id = order.Id
            };

            var result = await _orderHandler.RefundOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(order.Id);
            result.Data.Product.Id.Should().Be(product.Id);
            result.Message.Should().Be("Este pedido já está cancelado");
        }

        [Fact]
        public async Task RefundOrderRequest_DadoPedidoComStatusRefunded_EntaoDeveRetornarRespostaDeFalha()
        {
            string productTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = productTitle
            };

            string userId = _faker.Person.UserName;
            Order order = new()
            {
                UserId = userId,
                Product = product,
                Status = EOrderStatus.Refunded
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            RefundOrderRequest request = new()
            {
                UserId = userId,
                Id = order.Id
            };

            var result = await _orderHandler.RefundOrderAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(order.Id);
            result.Data.Product.Id.Should().Be(product.Id);
        }

        [Fact]
        public async Task RefundOrderRequest_DadoUmPedidoPagoComRequestCorreto_EntaoDeveMudarStatusParaRefundedESalvarCorretamenteNoBanco()
        {
            string productTitle = _faker.Vehicle.Model();
            Product product = new()
            {
               Title = productTitle
            };

            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();

            string userId = _faker.Person.UserName;
            Order order = new()
            {
                UserId = userId,
                Product = product,
                ProductId = product.Id,
                Status = EOrderStatus.Paid
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            RefundOrderRequest request = new()
            {
                UserId = userId,
                Id = order.Id
            };


            
            DateTime dateTimeBeforeHandler = DateTime.UtcNow;
            var result = await _orderHandler.RefundOrderAsync(request);
            DateTime dateTimeAfterHandler = DateTime.UtcNow;
            _appDbContext.ChangeTracker.Clear();
    
            Order? updatedOrder = _appDbContext.Orders.Include(x => x.Product).Include(x => x.Voucher).SingleOrDefault(x => x.Id == order.Id && x.UserId == order.UserId);

            updatedOrder.Should().NotBeNull();
            updatedOrder!.Product.Id.Should().Be(product.Id);
            updatedOrder.Id.Should().Be(order.Id);
            updatedOrder.Status.Should().Be(EOrderStatus.Refunded);
            updatedOrder.UpdatedAt.Should().BeAfter(dateTimeBeforeHandler);
            updatedOrder.UpdatedAt.Should().BeBefore(dateTimeAfterHandler);
        }

        [Fact]
        public async Task RefundOrderAsync_DadoUmPedidoPagoComRequestCorreto_EntaoDeveRetornarUmaRespostaDeSucesso()
        {
            string productTitle = _faker.Vehicle.Model();
            Product product = new()
            {
                Title = productTitle
            };

            string userId = _faker.Person.UserName;
            Order order = new()
            {
                UserId = userId,
                Product = product,
                Status = EOrderStatus.Paid
            };

            _appDbContext.Orders.Add(order);
            _appDbContext.SaveChanges();

            RefundOrderRequest request = new()
            {
                UserId = userId,
                Id = order.Id
            };


            _appDbContext.ChangeTracker.Clear();
            DateTime dateTimeBeforeHandler = DateTime.UtcNow;
            var result = await _orderHandler.RefundOrderAsync(request);
            DateTime dateTimeAfterHandler = DateTime.UtcNow;

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Product.Id.Should().Be(product.Id);
            result.Data.Id.Should().Be(order.Id);
            result.Data.Status.Should().Be(EOrderStatus.Refunded);
            result.Data.UpdatedAt.Should().BeAfter(dateTimeBeforeHandler);
            result.Data.UpdatedAt.Should().BeBefore(dateTimeAfterHandler);
        }


        #endregion

    }
}