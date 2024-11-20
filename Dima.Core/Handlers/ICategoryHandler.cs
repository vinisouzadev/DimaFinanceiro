using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;

namespace Dima.API.Handlers
{
    public interface ICategoryHandler
    {
        Task<Response<Category?>> CreateAsync(CreateCategoryRequest request);
        Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request);
        Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request);
        Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoryRequest request);
        Task<Response<Category?>> GetByIdAsync(GetByIdCategoryRequest request);
    
}
}
