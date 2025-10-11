using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProductDbContext _productDbContext;
        public CategoryRepository(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _productDbContext.Categories
                                      .AsNoTracking()
                                      .Include(c => c.SubCategories)
                                      .ToListAsync();
        }
        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _productDbContext.Categories.Where(c => c.Id == categoryId)
                                   .AsNoTracking()
                                   .Include(c => c.SubCategories)
                                   .FirstOrDefaultAsync();
        }

        public async Task<Category?> AddCategoryAsync(Category category)
        {
            await _productDbContext.Categories.AddAsync(category);
            await _productDbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            _productDbContext.Categories.Update(category);
            await _productDbContext.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _productDbContext.Categories.FindAsync(categoryId);

            if (category != null)
            {
                category.IsActive = false;
                await _productDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
