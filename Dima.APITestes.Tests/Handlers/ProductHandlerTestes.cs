using Bogus;
using Dima.API.Data;
using Dima.API.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Dima.APITestes.Tests.Handlers
{
    [Trait("Category", "Handlers")]
    public class ProductHandlerTestes
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly AppDbContext _context;

        private readonly ProductHandler _handler;

        public ProductHandlerTestes()
        {
            DbContextOptionsBuilder<AppDbContext> options = new();
            options.UseInMemoryDatabase("ProductHandlerTestes");
            _context = new(options.Options);

            _handler = new(_context);

        }


        #region GetAllProductsAsync

        [Fact]
        public async Task GetAllProductsAsync_DadoUmaListaDePedidos_EntaoDeveRetornarUmaRespostaComEssaLista()
        {
            string productTitle = _faker.Vehicle.Model();
            Product firstProduct = new()
            {
                Title = productTitle,
                IsActive = true
            };

            Product secondProduct = new()
            {
                Title = productTitle,
                IsActive = true
            };

            _context.Products.Add(firstProduct);
            _context.Products.Add(secondProduct);
            _context.SaveChanges();

            GetAllProductsRequest request = new();

            var result = await _handler.GetAllProductsAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.All(x => x.Title == productTitle);
            result.Data!.Count.Should().Be(2);
        }

        #endregion

        #region GetProductBySlugAsync

        [Fact]
        public async Task GetProductBySlugAsync_DadoUmPedidoComSlugIncorreto_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string productTitle = _faker.Vehicle.Model();
            string productSlug = _faker.Vehicle.Manufacturer();

            Product product = new()
            {
                Title = productTitle,
                IsActive = true
            };

            string incorrectlyProductSlug = _faker.Vehicle.Vin();

            _context.Products.Add(product);

            GetProductBySlugRequest request = new()
            {
                Slug = incorrectlyProductSlug
            };

            var result = await _handler.GetProductBySlugAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Produto não encontrado");

        }

        [Fact]
        public async Task GetProductBySlug_DadoProdutoInativo_EntaoDeveRetornarNulo()
        {
            string productTitle = _faker.Vehicle.Model();
            string productSlug = _faker.Vehicle.Manufacturer();

            Product product = new()
            {
                Title = productTitle,
                IsActive = false,
                Slug = productSlug
            };

            string incorrectlyProductSlug = _faker.Vehicle.Vin();

            _context.Products.Add(product);

            GetProductBySlugRequest request = new()
            {
                Slug = incorrectlyProductSlug
            };

            var result = await _handler.GetProductBySlugAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Produto não encontrado");
        }

        [Fact]
        public async Task GetProductBySlugAsync_DadoUmSlugDeUmProdutoAtivo_EntaoDeveRetornarUmaRespostaDeSucessoComOProduto()
        {
            string productTitle = _faker.Vehicle.Model();
            string productSlug = _faker.Vehicle.Manufacturer();
            Product product = new()
            {
                Title = productTitle,
                Slug = productSlug,
                IsActive = true
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            GetProductBySlugRequest request = new()
            {
                Slug = product.Slug
            };

            var result = await _handler.GetProductBySlugAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(product.Id);
        }

        #endregion

    }
}
