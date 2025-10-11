using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            var products = await _repository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<ProductDTO?> GetByIdAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;
            ProductDTO productDTO = _mapper.Map<ProductDTO>(product);
            return productDTO;
        }

        public async Task<ProductDTO?> AddAsync(ProductCreateDTO productDto)
        {
            Product product = _mapper.Map<Product>(productDto);
            product.Id = Guid.NewGuid();
            product.CreatedOn = DateTime.UtcNow;
            product.ModifiedOn = null;
            // Generate SKU dynamically before saving
            product.SKU = await  GenerateProductSKUAsync(product);
            var createdProduct = await _repository.AddAsync(product);
            return createdProduct == null ? null : _mapper.Map<ProductDTO>(createdProduct);
        }      
        public async Task<ProductDTO?> UpdateAsync(ProductUpdateDTO productDto)
        {
            var existingProduct = await _repository.GetByIdAsync(productDto.Id);
            if (existingProduct == null)
                return null;
            var product = _mapper.Map<Product>(productDto);
            product.CreatedOn = existingProduct.CreatedOn; // keep original created time
            product.ModifiedOn = DateTime.UtcNow;

            // Generate SKU dynamically before saving
            product.SKU = await GenerateProductSKUAsync(product);

            var updatedProduct = await _repository.UpdateAsync(product);
            return updatedProduct == null ? null : _mapper.Map<ProductDTO>(updatedProduct);

        }
        public async Task<bool> DeleteAsync(Guid productId)
        {
           return  await _repository.DeleteAsync(productId);
        }


        public async Task<List<ProductDTO>> SearchAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice, int pageNumber = 1, int pageSize = 20)
        {
           var products= await  _repository.SearchAsync(searchTerm, categoryId, minPrice, maxPrice, pageNumber, pageSize);
            return _mapper.Map<List<ProductDTO>>(products);

        }


        private async Task<string> GenerateProductSKUAsync(Product product)
        {
            // Get category by Id to fetch Category Name
            var category = await _categoryRepository.GetCategoryByIdAsync(product.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Category not found.");
            }

            string categoryPart = GetFirst3Letters(category.Name);
            string productPart = GetFirst3Letters(product.Name);
            string yearPart = DateTime.UtcNow.Year.ToString();

            // Get last 4 chars of Product GUID (without hyphens, uppercase)
            string guidSuffix = product.Id.ToString("N").Substring(28, 4).ToUpper();

            // Combine Parts to SKU
            return $"{categoryPart}-{productPart}-{yearPart}-{guidSuffix}";
        }
        private string GetFirst3Letters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "XXX"; // fallback

            return new string(input.Where(char.IsLetterOrDigit).Take(3).ToArray()).ToUpper().PadRight(3, 'X');
        }

        public async Task<List<ProductDTO>> GetByIdsAsync(IEnumerable<Guid> productIds)
        {
            var products = await _repository.GetByIdsAsync(productIds);
            return _mapper.Map<List<ProductDTO>>(products);
        }

    }
}
