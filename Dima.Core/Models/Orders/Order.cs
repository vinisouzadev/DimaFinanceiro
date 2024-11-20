using Dima.Core.Enums;

namespace Dima.Core.Models.Orders
{
    public class Order
    {
        public long Id { get; set; }

        public string OrderCode { get; set; } = Guid.NewGuid().ToString("N")[..8];

        public string? ExternalReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public EPaymentGateway Gateway { get; set; } = EPaymentGateway.Stripe;

        public EOrderStatus Status { get; set; } = EOrderStatus.WaitingPayment;

        public long ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public long? VoucherId { get; set; }

        public Voucher? Voucher { get; set; }

        public string UserId { get; set; } = string.Empty;

        public decimal Total => Product.Price - (Voucher?.Amount ?? 0);
    }
}
