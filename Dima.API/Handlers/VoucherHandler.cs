using Dima.API.Data;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Dima.API.Handlers
{
    public class VoucherHandler(AppDbContext context) : IVoucherHandler
    {
        private readonly AppDbContext _context = context;
        public async Task<Response<Voucher?>> GetVoucherByCodeAsync(GetVoucherByCodeRequest request)
        {
            try
            {
                var voucher = await _context.Vouchers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.IsActive && v.VourcherCode == request.Code );

                return voucher is null
                    ? new Response<Voucher?>(null, 404, "Voucher não encontrado.")
                    : new Response<Voucher?>(voucher);
            }
            catch
            {
                return new Response<Voucher?>(null, 500, "Não foi possível identificar seu voucher.");
            }
        }
    }
}
