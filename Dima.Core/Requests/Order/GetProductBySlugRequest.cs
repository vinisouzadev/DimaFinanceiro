namespace Dima.Core.Requests.Order
{
    public class GetProductBySlugRequest : Request
    {
        public string Slug { get; set; } = string.Empty;
    }
}
