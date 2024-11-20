namespace Dima.Core.Requests.Order
{
    public class GetVoucherByCodeRequest : Request
    {
        public string Code { get; set; } = string.Empty;
    }
}
