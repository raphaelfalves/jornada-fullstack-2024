using Fina.Api.Data;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers
{
    public class CategoryHandler(AppDbContext context) : ICategoryHandler
    {
        public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
        {

            try
            {
                var category = new Category()
                {
                    UserId = request.UserId,
                    Title = request.Title,
                    Description = request.Description
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // usar algo como serilog para não deixar falhar silenciosamente em produção
                return new(null, 500, $"Não foi possível criar {nameof(Category)}.");
            }

            return new Response<Category?>(null, 201, $"{nameof(Category)} Criada com sucesso.");
        }

        public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (category == null)
                return new(null, 404, $"{nameof(Category)} não encontrado.");

            try
            {

                context.Remove(category);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new(null, 500, $"Não foi possível deletar {nameof(Category)}.");
            }

            return new Response<Category?>(category, 200, $"{nameof(Category)} Deletado com sucesso.");
        }

        public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request)
        {
            try
            {
                var query = context
                    .Categories
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .OrderBy(x => x.Title);

                var categories = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var count = await query.CountAsync();

                return new PagedResponse<List<Category>?>(
                    categories,
                    count,
                    request.PageNumber,
                    request.PageSize);
            }
            catch
            {
                return new PagedResponse<List<Category>?>(null, 500, "Não foi possível consultar as categorias");
            }
        }

        public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
        {
            var category = await context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (category == null)
                return new(null, 404, $"{nameof(Category)} não encontrado.");

            return new Response<Category?>(category, 200, $"{nameof(Category)} encontrada.");
        }

        public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (category == null)
                return new(null, 404, $"{nameof(Category)} não encontrado.");

            try
            {
                category.Title = request.Title;
                category.Description = request.Description;

                context.Update(category);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new(null, 500, $"Não foi possível criar {nameof(Category)}.");
            }

            return new Response<Category?>(category,200, $"{nameof(Category)} Atualizo com sucesso.");
        }
    }
}
