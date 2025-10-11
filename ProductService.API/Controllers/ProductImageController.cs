using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTOs;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;

namespace ProductService.API.Controllers
{
    [Route("api/product-images")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _imageService;
        private readonly ILogger<ProductImageController> _logger;

        public ProductImageController(IProductImageService imageService, ILogger<ProductImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        [HttpGet("by-product{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetByProductId(Guid id)
        {
            try
            {
                var images = await _imageService.GetByProductIdAsync(id);
                return Ok(APIResponse<List<ProductImageDTO>>.SuccessResponse(images));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByProductId");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));
            }

        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var image = await _imageService.GetByIdAsync(id);
                if (image == null)
                    return NotFound(APIResponse<string>.FailResponse($"Image with id '{id}' not found"));

                return Ok(APIResponse<ProductImageDTO>.SuccessResponse(image));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add([FromBody] ProductImageCreateDTO createDTO)
        {
            try
            {
                var image = await _imageService.AddAsync(createDTO);
                if (image == null)
                {
                    return StatusCode(500, APIResponse<string>.FailResponse("Failed to add image"));
                }
                return Ok(APIResponse<ProductImageDTO>.SuccessResponse(image, "Image added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Add");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));

            }
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Update([FromBody] ProductImageUpdateDTO updateDto)
        {
            try
            {
                var result = await _imageService.UpdateAsync(updateDto);
                if (result == null)
                {
                    return StatusCode(404, APIResponse<string>.FailResponse($"Image with id '{updateDto.Id}' not found"));
                }
                return Ok(APIResponse<ProductImageDTO>.SuccessResponse(result, "Image updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));

            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            try
            {
                await _imageService.DeleteAsync(id);
                return Ok(APIResponse<string>.SuccessResponse("Image deleted successfully"));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in delete product image");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));
            }
        }
    }
}
