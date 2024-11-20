using Bogus;
using Dima.API.Data;
using Dima.API.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Dima.APITestes.Tests.Handlers
{
    [Trait("Category", "Handlers")]
    public class VoucherHandlerTestes
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly AppDbContext _context;

        private readonly VoucherHandler _handler;

        public VoucherHandlerTestes()
        {
            DbContextOptionsBuilder<AppDbContext> options = new();
            options.UseSqlite("Filename=:memory:");
            _context = new(options.Options);

            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _handler = new(_context);
        }

        [Fact]
        public async Task GetVoucherByCodeAsync_DadoUmVoucherInativo_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string voucherCode = _faker.Random.Utf16String();
            string voucherTitle = _faker.Vehicle.Model();
            bool isActive = false;
            decimal voucherAmount = _faker.Random.Decimal();
            Voucher voucher = new() 
            {
                Title = voucherTitle,
                Amount = voucherAmount,
                IsActive = isActive,
                VourcherCode = voucherCode
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            GetVoucherByCodeRequest request = new()
            {
                Code = voucherCode
            };

            var result = await _handler.GetVoucherByCodeAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Voucher não encontrado.");

        }

        [Fact]
        public async Task GetVoucherByCodeAsync_DadoUmVoucherCodeIncorreto_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string voucherCode = _faker.Random.Utf16String();
            string voucherTitle = _faker.Vehicle.Model();
            bool isActive = true;
            decimal voucherAmount = _faker.Random.Decimal();
            Voucher voucher = new()
            {
                Title = voucherTitle,
                Amount = voucherAmount,
                IsActive = isActive,
                VourcherCode = voucherCode
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            string incorretlyVoucherCode = _faker.Random.Utf16String();
            GetVoucherByCodeRequest request = new()
            {
                Code = incorretlyVoucherCode
            };

            var result = await _handler.GetVoucherByCodeAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Voucher não encontrado.");
        }

        [Fact]
        public async Task GetVoucherByCodeAsync_DadoUmVoucherValido_EntaoDeveRetornarUmaRespostaDeSucessoComOVoucher()
        {
            string voucherCode = _faker.Random.Utf16String();
            string voucherTitle = _faker.Vehicle.Model();
            bool isActive = true;
            decimal voucherAmount = _faker.Random.Decimal();
            Voucher voucher = new()
            {
                Title = voucherTitle,
                Amount = voucherAmount,
                IsActive = isActive,
                VourcherCode = voucherCode
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            GetVoucherByCodeRequest request = new()
            {
                Code = voucherCode
            };

            var result = await _handler.GetVoucherByCodeAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(voucher.Id);
        }

    }
}
