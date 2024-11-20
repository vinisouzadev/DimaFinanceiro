using Dima.API.Data;
using Dima.Core.Handlers;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;
using Dima.Core.Enums;
using Dima.Core.Requests.Stripe;
using Dima.Core.Models.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Dima.API.Handlers
{
    public class OrderHandler(AppDbContext context, IStripeHandler stripeHandler) : IOrderHandler
    {
        private readonly AppDbContext _context = context;

        private readonly IStripeHandler _stripeHandler = stripeHandler;

        public async Task<Response<Order?>> CancelOrderAsync(CancelOrderRequest request)
        {
            Order? order;
            try
            {
                order = await _context.Orders
                    .Include(x => x.Voucher)
                    .Include(x => x.Product)
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (order is null)
                    return new Response<Order?>(null, 404, "Pedido não encontrado");
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível cancelar seu pedido");
            }

            if (order.Status != EOrderStatus.WaitingPayment)
                return new Response<Order?>(null, 400, "Este pedido não pode ser cancelado");

            order.Status = EOrderStatus.Canceled;
            order.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível cancelar seu pedido");
            }

            return new Response<Order?>(order, 200, $"Pedido {order.OrderCode} cancelado com sucesso");
        }

        public async Task<Response<Order?>> CreateOrderAsync(CreateOrderRequest request)
        {
            Product? product;
            try
            {
                product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IsActive && x.Id == request.ProductId);

                if (product is null)
                    return new Response<Order?>(null, 404, "Produto não encontrado.");

                _context.Attach(product);
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível encontrar o produto.");
            }

            Voucher? voucher = null;
            try
            {
                if (request.VoucherId is not null)
                {
                    voucher = await _context.Vouchers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.IsActive && x.Id == request.VoucherId);

                    if (voucher is null)
                        return new Response<Order?>(null, 404, "Voucher não encontrado.");

                    voucher.IsActive = false;
                    _context.Vouchers.Update(voucher);

                }
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível identificar o Voucher");
            }

            Order? order = new Order
            {
                UserId = request.UserId,
                Product = product,
                Voucher = voucher,
                ProductId = request.ProductId,
                VoucherId = request.VoucherId

            };

            try
            {
                await _context.AddAsync(order);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível criar o pedido");
            }

            return new Response<Order?>(order, 201, $"Pedido {order.OrderCode} criado com sucesso!");

        }

        public async Task<Response<List<Order>?>> GetAllOrdersAsync(GetAllOrdersRequest request)
        {
            try
            {
                var query = _context.Orders
                    .AsNoTracking()
                    .Include(x => x.Product)
                    .Include(x => x.Voucher)
                    .Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.CreatedAt);

                var orders = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var count = await query.CountAsync();

                return new PagedResponse<List<Order>?>(orders, count, request.PageNumber, request.PageSize);
            }
            catch
            {
                return new PagedResponse<List<Order>?>(null, 500, "Não foi possível identificar seus pedidos");
            }
        }

        public async Task<Response<Order?>> GetOrderByCodeAsync(GetOrderByCodeRequest request)
        {
            try
            {
                var order = await _context.Orders
                    .AsNoTracking()
                    .Include(x => x.Product)
                    .Include(x => x.Voucher)
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.OrderCode == request.Code);

                return order is null
                    ? new Response<Order?>(null, 404, "Pedido não encontrado")
                    : new Response<Order?>(order);
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível identificar seu pedido");
            }

        }

        public async Task<Response<Order?>> PayOrderAsync(PayOrderRequest request)
        {
            Order? order;
            try
            {
                order = await _context.Orders
                    .Include(x => x.Voucher)
                    .Include(x => x.Product)
                    .FirstOrDefaultAsync(x => x.OrderCode == request.OrderNumber && x.UserId == request.UserId);

                if (order is null)
                    return new Response<Order?>(null, 404, "Pedido não encontrado");
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível identificar o pedido");
            }

            switch (order.Status)
            {
                case EOrderStatus.Canceled:
                    return new Response<Order?>(order, 400, "Este pedido já foi cancelado e não pode ser pago");

                case EOrderStatus.Paid:
                    return new Response<Order?>(order, 400, "Este pedido já foi pago");

                case EOrderStatus.Refunded:
                    return new Response<Order?>(order, 400, "Este pedido já foi reembolsado e não pode ser pago novamente.");

                case EOrderStatus.WaitingPayment:
                    break;

                default:
                    return new Response<Order?>(order, 400, "Não foi possível realizar o pagamento do pedido.");
            }

            try
            {
                var getTransactionsRequest = new GetTransactionsByOrderNumberRequest
                {
                    OrderNumber = order.OrderCode
                };

                var result = await _stripeHandler.GetTransactionsByOrderNumberAsync(getTransactionsRequest);

                if (!result.IsSuccess || result.Data is null)
                    return new Response<Order?>(null, 500, "Não foi possível encontrar este pagamento");
                if (result.Data.Any(x => x.Refunded))
                    return new Response<Order?>(null, 400, "Este pedido já foi estornado");
                if (!result.Data.Any(x => x.Paid))
                    return new Response<Order?>(null, 400, "Este pedido ainda não foi pago");


                request.ExternalReference = result.Data[0].Id;
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível dar baixa no seu pedido! Entre em contato com nosso suporte!");
            }

            order.Status = EOrderStatus.Paid;
            order.ExternalReference = request.ExternalReference;
            order.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return new Response<Order?>(order, 500, "Falha ao tentar pagar o pedido");
            }

            return new Response<Order?>(order, 200, "Pedido pago com sucesso!");
        }

        public async Task<Response<Order?>> RefundOrderAsync(RefundOrderRequest request)
        {
            Order? order;
            try
            {
                order = await _context.Orders
                    .Include(x => x.Voucher)
                    .Include(x => x.Product)
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (order is null)
                    return new Response<Order?>(null, 404, "Pedido não encontrado.");
            }
            catch
            {
                return new Response<Order?>(null, 500, "Não foi possível identificar o pedido");
            }

            switch (order.Status)
            {
                case EOrderStatus.Paid:
                    break;

                case EOrderStatus.WaitingPayment:
                    return new Response<Order?>(order, 400, "Este pedido ainda não foi pago e por isso não pode ser reembolsado");

                case EOrderStatus.Canceled:
                    return new Response<Order?>(order, 400, "Este pedido já está cancelado");

                case EOrderStatus.Refunded:
                    return new Response<Order?>(order, 400, "Este pedido já foi reembolsado");
            }

            order.Status = EOrderStatus.Refunded;
            order.UpdatedAt = DateTime.UtcNow;
            

            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return new Response<Order?>(order, 500, "Falha ao reembolsar pagamento");
            }

            return new Response<Order?>(order, 200, "Reembolso de pagamento realizado com sucesso!");

        }
    }
}
