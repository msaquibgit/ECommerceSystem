using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Interface
{
    public interface IProductService
    {
        Task<ProductDTO?> GetByIdAsync(Guid id);
        Task<List<ProductDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
        Task<List<ProductDTO>> SearchAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, int pageNumber = 1, int pageSize = 20);
        Task<ProductDTO?> AddAsync(ProductCreateDTO productDto);
        Task<ProductDTO?> UpdateAsync(ProductUpdateDTO productDto);
        Task<bool> DeleteAsync(Guid productId);
        Task<List<ProductDTO>> GetByIdsAsync(IEnumerable<Guid> productIds);

    }
}
