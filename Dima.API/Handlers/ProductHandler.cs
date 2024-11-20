using Dima.API.Data;
using Dima.Core.Handlers;
using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.API.Handlers
{
    public class ProductHandler(AppDbContext context) : IProductHandler
    {
        private readonly AppDbContext _context = context;

        public async Task<PagedResponse<List<Product>?>> GetAllProductsAsync(GetAllProductsRequest request)
        {
            try
            {
                var query = _context.Products
                    .AsNoTracking()
                    .Where(q => q.IsActive)
                    .OrderBy(q => q.Title);

                var products = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var count = await query.CountAsync();

                return new PagedResponse<List<Product>?>(products, count, request.PageNumber, request.PageSize);
            }
            catch
            {
                return new PagedResponse<List<Product>?>(null, 500, "Não foi possível identificar os produtos");
            }
        }

        public async Task<Response<Product?>> GetProductBySlugAsync(GetProductBySlugRequest request)
        {
            try
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Slug == request.Slug && p.IsActive);

                return product is null
                    ? new Response<Product?>(null, 404, "Produto não encontrado")
                    : new Response<Product?>(product);
            }
            catch
            {
                return new Response<Product?>(null, 500, "Não foi possível identificar seu produto");
            }
        }
    }
}
