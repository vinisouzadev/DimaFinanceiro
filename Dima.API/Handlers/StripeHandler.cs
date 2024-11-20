using Dima.Core.Common;
using Dima.Core.Handlers;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using Dima.Core.Responses.Stripe;
using Stripe;
using Stripe.Checkout;
using System.Net.WebSockets;

namespace Dima.API.Handlers
{
    public class StripeHandler : IStripeHandler
    {

        public async Task<Response<List<StripeTransactionResponse>>> GetTransactionsByOrderNumberAsync(GetTransactionsByOrderNumberRequest request)
        {
            var options = new ChargeSearchOptions
            {
                Query = $"metadata['order']: '{request.OrderNumber}'"
            };

            var service = new ChargeService();
            var result = await service.SearchAsync(options);

            if (result.Data.Count == 0)
                return new Response<List<StripeTransactionResponse>>(null, 404, "Não foi possível identificar nenhuma transação");

            var data = new List<StripeTransactionResponse>();

            foreach (var transaction in result.Data) 
            {
                data.Add(new StripeTransactionResponse
                {
                    Id = transaction.Id,
                    Email = transaction.BillingDetails.Email,
                    Status = transaction.Status,
                    Amount = transaction.Amount,
                    AmountCaptured = transaction.AmountCaptured,
                    Paid = transaction.Paid,
                    Refunded = transaction.Refunded
                });
            }

            return new Response<List<StripeTransactionResponse>>(data);
        }

        public async Task<Response<string?>> CreateSessionAsync(CreateSessionRequest request)
        {
            var options = new SessionCreateOptions
            {
                CustomerEmail = request.UserId,
                PaymentIntentData = new SessionPaymentIntentDataOptions()
                {
                    Metadata = new Dictionary<string, string>
                    {
                        {"order", request.OrderNumber }

                    }
                },
                PaymentMethodTypes =
                [
                    "card"
                ],
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            Currency = "BRL",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = request.ProductTitle,
                                Description = request.ProductDescription,

                            },
                            UnitAmount = request.OrderTotal
                        },
                        Quantity = 1
                    }
                ],
                Mode = "payment",
                SuccessUrl = $"{Configuration.FrontendUrl}/pedidos/{request.OrderNumber}/confirmar",
                CancelUrl = $"{Configuration.FrontendUrl}/pedidos/{request.OrderNumber}/cancelar"
            };

            var services = new SessionService();

            var session = await services.CreateAsync(options);

            return new Response<string?>(session.Id);
        }
    }
}
