using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<DiscountDTO>> GetByProductIdAsync(Guid productId)
        {
            var discounts = await _repository.GetDiscountsByProductIdAsync(productId);
            return _mapper.Map<List<DiscountDTO>>(discounts);
        }

        public async Task<DiscountDTO?> GetActiveDiscountByProductIdAsync(Guid productId)
        {
            var discount = await _repository.GetActiveDiscountByProductIdAsync(productId);
            return discount == null ? null : _mapper.Map<DiscountDTO>(discount);
        }

        public async Task<DiscountDTO?> AddAsync(DiscountCreateDTO createDto)
        {
            var discount = _mapper.Map<Discount>(createDto);
            discount.Id = Guid.NewGuid();
            discount.CreatedOn = DateTime.UtcNow;
            discount.ModifiedOn = null;

            var createdDiscount = await _repository.AddDiscountAsync(discount);
            return createdDiscount == null ? null : _mapper.Map<DiscountDTO>(createdDiscount);
        }

        public async Task<DiscountDTO?> UpdateAsync(DiscountUpdateDTO updateDto)
        {
            var discountToUpdate = await _repository.GetActiveDiscountByProductIdAsync(updateDto.Id);
            if (discountToUpdate == null)
                return null;

            var discount = _mapper.Map<Discount>(updateDto);
            discount.CreatedOn = discountToUpdate.CreatedOn;
            discount.ModifiedOn = DateTime.UtcNow;

            var updatedDiscount = await _repository.UpdateDiscountAsync(discount);
            return updatedDiscount == null ? null : _mapper.Map<DiscountDTO>(updatedDiscount);
        }

        public async Task DeleteAsync(Guid discountId)
        {
            await _repository.DeleteDiscountAsync(discountId);
        }
    }
}
