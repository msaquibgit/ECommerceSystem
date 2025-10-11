using ProductService.Application.DTOs;

namespace ProductService.Application.Interface
{
    public interface ICategoryService
    {
        Task<CategoryDTO?> GetByIdAsync(Guid id);
        Task<List<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> AddAsync(CategoryCreateDTO createDto);
        Task<CategoryDTO?> UpdateAsync(CategoryUpdateDTO updateDto);
        Task<bool> DeleteAsync(Guid id);

    }
}
