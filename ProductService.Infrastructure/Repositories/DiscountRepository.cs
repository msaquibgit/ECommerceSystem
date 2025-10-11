using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Repositories
{
    public class DiscountRepository:IDiscountRepository
    {
        private readonly ProductDbContext _dbContext;
        public DiscountRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Discount>> GetDiscountsByProductIdAsync(Guid productId)
        {
            return await _dbContext.Discounts
                .Where(d => d.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Discount?> GetActiveDiscountByProductIdAsync(Guid productId)
        {
            return await _dbContext.Discounts
                .Where(d => d.ProductId == productId && d.IsActive)
                .OrderByDescending(d => d.CreatedOn)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Discount?> AddDiscountAsync(Discount discount)
        {
            var activeDiscount = await _dbContext.Discounts.Where(x => x.IsActive == true && x.ProductId == discount.ProductId)
                .ToListAsync();
            if (activeDiscount.Any())
            {
                foreach (var dis in activeDiscount)
                {
                    dis.IsActive = false;
                    await _dbContext.SaveChangesAsync();

                }
            }
            await _dbContext.Discounts.AddAsync(discount);
            await _dbContext.SaveChangesAsync();
            return discount;
        }
        public async Task<Discount?> UpdateDiscountAsync(Discount discount)
        {
            _dbContext.Discounts.Update(discount);
            await _dbContext.SaveChangesAsync();
            return discount;
        }

        public async Task<bool> DeleteDiscountAsync(Guid discountId)
        {
            var discount = await _dbContext.Discounts.FindAsync(discountId);
            if (discount != null)
            {
                discount.IsActive = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
