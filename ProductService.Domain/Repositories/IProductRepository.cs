using ProductService.Domain.Entities;

namespace ProductService.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId);
        Task<List<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<List<Product>> SearchAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, int pageNumber = 1, int pageSize = 10);
        Task<Product?> AddAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid productId);
        Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds);

    }
}
