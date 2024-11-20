namespace Dima.Core.Requests.Order
{
    public class GetOrderByCodeRequest : Request
    {
        public string Code { get; set; } = string.Empty;
    }
}
