using ProductService.Application.DTOs;

namespace ProductService.Application.Interface
{
    public interface IDiscountService
    {
        Task<List<DiscountDTO>> GetByProductIdAsync(Guid productId); //Active and Inactive
        Task<DiscountDTO?> GetActiveDiscountByProductIdAsync(Guid productId);
        Task<DiscountDTO?> AddAsync(DiscountCreateDTO createDto);
        Task<DiscountDTO?> UpdateAsync(DiscountUpdateDTO updateDto);
        Task DeleteAsync(Guid discountId);

    }
}
