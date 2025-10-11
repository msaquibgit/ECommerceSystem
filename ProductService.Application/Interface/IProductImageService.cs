using ProductService.Application.DTOs;

namespace ProductService.Application.Interface
{
    public interface IProductImageService
    {
        Task<List<ProductImageDTO>> GetByProductIdAsync(Guid productId);
        Task<ProductImageDTO?> GetByIdAsync(Guid imageId);
        Task<ProductImageDTO?> AddAsync(ProductImageCreateDTO createDto);
        Task<ProductImageDTO?> UpdateAsync(ProductImageUpdateDTO updateDto);
        Task DeleteAsync(Guid imageId);

    }
}
