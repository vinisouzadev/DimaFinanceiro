using Dima.Core.Models.Orders;
using Dima.Core.Requests.Order;
using Dima.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dima.Core.Handlers
{
    public interface IProductHandler
    {
        Task<Response<Product?>> GetProductBySlugAsync(GetProductBySlugRequest request);

        Task<PagedResponse<List<Product>?>> GetAllProductsAsync(GetAllProductsRequest request);

    }
}
