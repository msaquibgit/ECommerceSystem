using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _productDbContext;

        public ProductRepository(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        public async Task<List<Product>> SearchAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, int pageNumber = 1, int pageSize = 10)
        {
            var query = _productDbContext.Products
                                       .Include(cat => cat.Category)
                                       .Include(img => img.ProductImages)
                                       .Include(dis => dis.Discounts)
                                       .Include(rev => rev.Reviews)
                                       .AsQueryable();
            if(!string.IsNullOrWhiteSpace(searchTerm) ) 
            {
                query = query.Where(x => x.Name.Contains(searchTerm) ||
                                     x.SKU.Contains(searchTerm) ||
                                     x.Description.Contains(searchTerm));
                if (categoryId.HasValue)
                {
                    query = query.Where(c => c.CategoryId == categoryId);
                }
                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice);                    
                }
                if (maxPrice.HasValue)
                {
                    query=query.Where(p=>p.Price <= maxPrice);
                }
               
            }
            return await query.OrderBy(p => p.Name)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }
        public async Task<Product?> AddAsync(Product product)
        {
            await  _productDbContext.Products.AddAsync(product);
            await _productDbContext.SaveChangesAsync();
            return product;
        }
        public async Task<List<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
          return await _productDbContext.Products.AsNoTracking()
                .Include(c=>c.Category)
                .Include(d=>d.Discounts)
                .Include(r=>r.Reviews)
                .Include(img=>img.ProductImages.Where(pi=>pi.IsPrimary))
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid productId)
        {
            return await _productDbContext.Products.Where(x=>x.Id==productId).AsNoTracking()
               .Include(c => c.Category)
               .Include(d => d.Discounts)
               .Include(r => r.Reviews)
               .Include(img => img.ProductImages.Where(pi => pi.IsPrimary))
               .FirstOrDefaultAsync();
        }
        public async Task<Product?> UpdateAsync(Product product)
        {
            _productDbContext.Products.Update(product);
             await _productDbContext.SaveChangesAsync();
            return product;
        }
        public async Task<bool> DeleteAsync(Guid productId)
        {
            var product = await _productDbContext.Products.Where(x => x.Id == productId)
                .FirstOrDefaultAsync();
            if(product != null)
            {
                product.IsActive = false;
                await _productDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds)
        {
            return await _productDbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary))
            .Include(p => p.Discounts)
            .Include(p => p.Reviews)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
        }
    }
}
