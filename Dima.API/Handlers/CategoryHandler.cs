using Dima.API.Data;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dima.API.Handlers
{
    public class CategoryHandler : ICategoryHandler
    {
        private readonly AppDbContext _context;

        public CategoryHandler(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
        {  
            try
            {
                 var category = new Category
                {
                    UserId = request.UserId,
                    Title = request.Title,
                    Description = request.Description
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return new Response<Category?>(category, 201, "Categoria criada com sucesso!");
            }
            catch
            {
                return new Response<Category?>(null, 500, "Não foi possível criar a sua categoria");
            }
        }

        public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
        {
            try
            {
                Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);

                if (category is null)
                {
                    return new Response<Category?>(null, 404, "Não foi possível identificar essa categoria");
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return new Response<Category?>(null, 204, "Categoria deletada com sucesso!");
            }
            catch
            {
                return new Response<Category?>(null, 500, "Não foi possível deletar essa categoria");
            }
        }

        public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoryRequest request)
        {
            try
            {
                var query = _context.Categories
                    .AsNoTracking()
                    .Where(q => q.UserId == request.UserId)
                    .OrderBy(o => o.Title);

                List<Category> categories = await query
                    .Skip(request.PageSize * (request.PageNumber - 1))
                    .Take(request.PageSize)
                    .ToListAsync();

                int count = await query.CountAsync();

                return new PagedResponse<List<Category>?>(categories, count, request.PageNumber, request.PageSize);


            }
            catch
            {
                return new PagedResponse<List<Category>?>(null, 500, "Houve uma falha ao mostrar suas categorias");
            }
        }

        public async Task<Response<Category?>> GetByIdAsync(GetByIdCategoryRequest request)
        {
            try
            {
                var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);

                return category is null
                    ? new Response<Category?>(null, 404, "Não foi possível identificar essa categoria")
                    : new Response<Category?>(category);
            }
            catch
            {
                return new Response<Category?>(null, 500, "Ocorreu um erro ao encontrar sua categoria");
            }
            
        }

        public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
        {
            try
            {
                Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);

                if (category is null)
                    return new Response<Category?>(null, 404, "Não foi possível identificar essa categoria");

                category.Title = request.Title;
                category.Description = request.Description;

                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new Response<Category?>(category, 200, "Categoria atualizada com sucesso");
            }
            catch
            {
                return new Response<Category?>(null, 500, "Não foi possível atualizar sua categoria");
            }
        }
    }
}
