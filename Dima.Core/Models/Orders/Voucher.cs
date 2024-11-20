namespace Dima.Core.Models.Orders
{
    public class Voucher
    {
        public long Id { get; set; }

        public string VourcherCode { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public decimal Amount { get; set; }
    }
}
