using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Services
{
    public class ReviewService:IReviewService
    {

        private readonly IReviewRepository _repository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<ReviewDTO>> GetByProductIdAsync(Guid productId)
        {
            var reviews = await _repository.GetReviewsByProductIdAsync(productId);
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO?> GetByIdAsync(Guid reviewId)
        {
            var review = await _repository.GetReviewByIdAsync(reviewId);
            return review == null ? null : _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO?> AddAsync(ReviewCreateDTO createDto)
        {
            var review = _mapper.Map<Review>(createDto);
            review.Id = Guid.NewGuid();
            review.CreatedOn = DateTime.UtcNow;
            review.ModifiedOn = null;

            var createdReview = await _repository.AddReviewAsync(review);
            return createdReview == null ? null : _mapper.Map<ReviewDTO>(createdReview);
        }

        public async Task<ReviewDTO?> UpdateAsync(ReviewUpdateDTO updateDto)
        {
            var existingReview = await _repository.GetReviewByIdAsync(updateDto.Id);
            if (existingReview == null)
                return null;

            var review = _mapper.Map<Review>(updateDto);
            review.CreatedOn = existingReview.CreatedOn;
            review.ModifiedOn = DateTime.UtcNow;

            var updatedReview = await _repository.UpdateReviewAsync(review);
            return updatedReview == null ? null : _mapper.Map<ReviewDTO>(updatedReview);
        }

        public async Task DeleteAsync(Guid reviewId)
        {
            await _repository.DeleteReviewAsync(reviewId);
        }
    }
}
