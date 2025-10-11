using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoryDTO?> AddAsync(CategoryCreateDTO createDto)
        {
            Category category = _mapper.Map<Category>(createDto);
            category.Id = Guid.NewGuid();
            category.CreatedOn = DateTime.UtcNow;
            category.ModifiedOn = null;

            var createdCategory = await _repository.AddCategoryAsync(category);
            return createdCategory==null?null : _mapper.Map<CategoryDTO>(createdCategory);

        }
        public async Task<List<CategoryDTO>> GetAllAsync()
        {
            List<Category> categories = await _repository.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryDTO>>(categories);
        }
        public async Task<CategoryDTO?> GetByIdAsync(Guid id)
        {
            Category? category = await _repository.GetCategoryByIdAsync(id);
            if(category==null) return null;
            return _mapper.Map<CategoryDTO>(category);
        }
        public async Task<CategoryDTO?> UpdateAsync(CategoryUpdateDTO updateDto)
        {
            
            var existingCategory = _repository.GetCategoryByIdAsync(updateDto.Id);
            if(existingCategory==null) return null;
            Category category = _mapper.Map<Category>(updateDto);
            var updatedCategory=  await _repository.UpdateCategoryAsync(category);
            return updatedCategory == null ? null : _mapper.Map<CategoryDTO>(updatedCategory);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
           return await _repository.DeleteCategoryAsync(id);
        }
    }
}
