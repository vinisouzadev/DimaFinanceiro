namespace Dima.Core.Requests.Order
{
    public class CreateOrderRequest : Request
    {
        public long ProductId { get; set; }

        public long? VoucherId { get; set; }
    }
}
