using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTOs;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;

namespace ProductService.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var products = await _productService.GetAllAsync(pageNumber, pageSize);
                return Ok(APIResponse<List<ProductDTO>>.SuccessResponse(products));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All Product");
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
                var product = await _productService.GetByIdAsync(id);
                if(product == null)
                {
                    return StatusCode(404, APIResponse<string>.FailResponse($"Product with id '{id}' not found"));
                }
                return Ok(APIResponse<ProductDTO>.SuccessResponse(product));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All Product");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));
            }
        }
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search
        (
            [FromQuery] string? searchTerm, [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice, [FromQuery] decimal maxPrice,
            [FromQuery] int pageNumber=1, [FromQuery] int pageSize=20
        )
        {
            try
            {
                var products = await _productService.SearchAsync(searchTerm,categoryId,minPrice,maxPrice,pageNumber,pageSize);
                return Ok(APIResponse<List<ProductDTO>>.SuccessResponse(products));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Serach error log");
                return StatusCode(500,APIResponse<string>.FailResponse("Internal server error."));
            }
        }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] ProductCreateDTO createDTO)
        {
            try
            {
                var result = await _productService.AddAsync(createDTO);
                if (result == null)
                {
                    return StatusCode(500, APIResponse<string>.FailResponse("Failed to create product"));
                }
                return Ok(APIResponse<ProductDTO>.SuccessResponse(result, "Product created successfully"));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error created in Add product");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));
            }
        }
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id,[FromBody] ProductUpdateDTO updateDTO)
        {
            try
            {
                var result = await _productService.UpdateAsync(updateDTO);
                if(result==null)
                {
                    return NotFound(APIResponse<string>.FailResponse($"Product with id '{updateDTO.Id}' not found"));
                }
                return Ok(APIResponse<ProductDTO>.SuccessResponse(result,"Product updated successfully"));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in product update");
                return StatusCode(500,APIResponse<string>.FailResponse("Internal server error"));
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return Ok(APIResponse<string>.SuccessResponse(string.Empty, "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Delete");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));
            }
        }

        [HttpPost("GetByIds")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductByIds([FromBody] List<Guid> productIds)
        {
            try
            {
                if (productIds == null || !productIds.Any())
                    return BadRequest("Product IDs list cannot be empty.");
                var products = await _productService.GetByIdsAsync(productIds);
                return Ok(APIResponse<List<ProductDTO>>.SuccessResponse(products));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProductByIds");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error"));
            }
        }

    }
}

