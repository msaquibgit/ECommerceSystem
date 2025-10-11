using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ProductDbContext _productDbContext;

        public InventoryRepository(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        public async Task IncreaseDecsreaseStockAsync(Guid productId, int stockQuantity)
        {
            var product = await _productDbContext.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity = stockQuantity;
                await _productDbContext.SaveChangesAsync();

            }
        }
        public async Task<bool> IsStockAvailableAsync(Guid productId, int quantity)
        {
            var product = await _productDbContext.Products
                                          .FirstOrDefaultAsync(prd => prd.Id == productId && prd.IsActive == true);

            if (product == null)
                return false;

            return product.StockQuantity >= quantity;

        }

        public async Task UpdateStockAsync(Guid productId, int stockQuantity)
        {
            var product = await _productDbContext.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity += stockQuantity;
                await _productDbContext.SaveChangesAsync();

            }
        }

        public async Task UpdateStockBulkAsync(Guid productId, int stockQuantity)
        {

            var product = await _productDbContext.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity = stockQuantity;
                await _productDbContext.SaveChangesAsync();

            }


        }
        public async Task IncreaseStockBulkAsync(IEnumerable<(Guid productId, int quantity)> stockUpdates)
        {
            foreach (var (productId, quantity) in stockUpdates)
            {
                var product = await _productDbContext.Products.FindAsync(productId);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {productId} not found.");

                product.StockQuantity += quantity;
                _productDbContext.Products.Update(product);
            }
            await _productDbContext.SaveChangesAsync();
        }
        public async Task DecreaseStockBulkAsync(IEnumerable<(Guid productId, int quantity)> stockUpdates)
        {
            foreach (var (productId, quantity) in stockUpdates)
            {
                var product = await _productDbContext.Products.FindAsync(productId);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {productId} not found.");

                if (product.StockQuantity < quantity)
                    throw new InvalidOperationException($"Insufficient stock for product ID {productId}");

                product.StockQuantity -= quantity;
                _productDbContext.Products.Update(product);
            }
            await _productDbContext.SaveChangesAsync();
        }


        public async Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds)
        {
            return await _productDbContext.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
        }
    }
}

