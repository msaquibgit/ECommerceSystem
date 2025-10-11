using ProductService.Domain.Entities;

namespace ProductService.Domain.Repositories
{
    public interface IInventoryRepository
    {
        Task<bool> IsStockAvailableAsync(Guid productId, int quantity);
        Task UpdateStockAsync(Guid productId, int stockQuantity);
        Task IncreaseDecsreaseStockAsync(Guid productId, int stockQuantity);
        Task UpdateStockBulkAsync(Guid productId, int stockQuantity);
        Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds);
        Task IncreaseStockBulkAsync(IEnumerable<(Guid productId, int quantity)> stockUpdates);
        Task DecreaseStockBulkAsync(IEnumerable<(Guid productId, int quantity)> stockUpdates);
    }
}
