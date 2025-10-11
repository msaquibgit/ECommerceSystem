using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ProductDbContext _productDbContext;
       

        public ProductImageRepository(ProductDbContext productDbContext)
        {   
            _productDbContext = productDbContext;
        }

        public async Task<List<ProductImage>> GetProductImagesAsync(Guid productId)
        {
            return await _productDbContext.ProductImages.Where(pi => pi.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductImage?> GetProductImageByIdAsync(Guid imageId)
        {
            //return await _productDbContext.ProductImages.AsNoTracking().Where(p => p.Id == imageId)
            //    .FirstOrDefaultAsync();


            return await _productDbContext.ProductImages.FindAsync(imageId);

        }

        public async Task<ProductImage?> AddProductImageAsync(ProductImage image)
        {
            await _productDbContext.ProductImages.AddAsync(image);
            await _productDbContext.SaveChangesAsync();
            return image;
        }
        public async Task<ProductImage?> UpdateProductImageAsync(ProductImage image)
        {
            _productDbContext.ProductImages.Update(image);
            await _productDbContext.SaveChangesAsync();
            return image;
        }


        public async Task<bool> RemoveProductImageAsync(Guid imageId)
        {
            var image = await _productDbContext.ProductImages.FirstOrDefaultAsync(img => img.Id == imageId);
            if(image != null)
            {
                image.IsActive = false; 
                await _productDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
