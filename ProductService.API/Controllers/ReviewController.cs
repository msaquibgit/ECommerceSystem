using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTOs;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;

namespace ProductService.API.Controllers
{
    [Route("api/product/review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }
        [HttpGet("by-product/{productId:guid}")]
        [ProducesResponseType(typeof(List<ReviewDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string),StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetReviewByProductId(Guid productId)
        {
            try
            {
              var reviews = await _reviewService.GetByProductIdAsync(productId);
                if (reviews == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Review not found."));
                }
                return Ok(APIResponse<List<ReviewDTO>>.SuccessResponse(reviews,"OK"));
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Internal server error in GetReviewByProductId");
                return StatusCode(500,APIResponse<string>.FailResponse("Internal server error."));
            }
        }
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ReviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetReviewById(Guid id)
        {
            try
            {
                var reviews = await _reviewService.GetByIdAsync(id);
                if (reviews == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Review not found."));
                }
                return Ok(APIResponse<ReviewDTO>.SuccessResponse(reviews, "OK"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error in GetReviewByProductId");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error."));
            }
        }
        [HttpPost("add")]
        [ProducesResponseType(typeof(ReviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(ReviewCreateDTO dto)
        {
            try
            {
                var review = await _reviewService.AddAsync(dto);
                if(review == null)
                {
                    return StatusCode(500, APIResponse<string>.FailResponse("Fail to add review"));
                }
                return Ok(APIResponse<ReviewDTO>.SuccessResponse(review,"Review added successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error in GetReviewByProductId");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error."));
            }
        }
        [HttpPut("update")]
        [ProducesResponseType(typeof(ReviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(ReviewUpdateDTO dto)
        {
            try
            {
                var review = await _reviewService.UpdateAsync(dto);
                if (review == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Record not found"));
                }
                return Ok(APIResponse<ReviewDTO>.SuccessResponse(review, "Review updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error in GetReviewByProductId");
                return StatusCode(500, APIResponse<string>.FailResponse("Error in update."));
            }
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _reviewService.DeleteAsync(id);
                return Ok(APIResponse<string>.SuccessResponse("Review deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error in delete review");
                return StatusCode(500, APIResponse<string>.FailResponse("Error in update."));
            }
        }
    }
}
