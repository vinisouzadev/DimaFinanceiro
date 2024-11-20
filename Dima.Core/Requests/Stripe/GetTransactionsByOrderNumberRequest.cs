namespace Dima.Core.Requests.Stripe
{
    public class GetTransactionsByOrderNumberRequest : Request
    {
        public string OrderNumber = string.Empty;
    }
}
