using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTOs;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using System.ComponentModel.DataAnnotations;

namespace ProductService.API.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;
        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet("avaiblity/{productId:guid}/{quantity:int}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckStock(Guid productId, int quantity)
        {
            try
            {
                var isAvailable = await _inventoryService.IsStockAvailableAsync(productId, quantity);
                if (isAvailable)
                {
                    return Ok(APIResponse<bool>.SuccessResponse(isAvailable));
                }
                return NotFound(APIResponse<string>.FailResponse("Not Found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server inside CheckStock ");
                return StatusCode(500, APIResponse<string>.FailResponse("Inernal Sever Error"));
            }
        }
        [HttpPost()]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<InventoryUpdateDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStock([FromBody] InventoryUpdateDTO Dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(APIResponse<InventoryUpdateDTO>.FailResponse("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            try
            {
                await _inventoryService.UpdateStockAsync(Dto);
                return Ok(APIResponse<string>.SuccessResponse(string.Empty, "Stock updated successfully."));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server inside CheckStock ");
                return StatusCode(500, APIResponse<string>.FailResponse("Inernal Sever Error"));
            }
        }

        [HttpPost("increase-stock")]
        public async Task<IActionResult> IncreaseStock([Required] Guid productId, [Required] int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    await _inventoryService.IncreaseStockAsync(productId, quantity);
                    return BadRequest(APIResponse<string>.FailResponse("quantity must be posititve."));
                }
                await _inventoryService.IncreaseStockAsync(productId, quantity);
                return Ok(APIResponse<string>.SuccessResponse("Stock increases."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DecreaseInventory");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error."));
            }
        }
        [HttpPost("decrease-stock")]
        public async Task<IActionResult> DecreaseStock([FromQuery] Guid productId, [FromQuery] int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return BadRequest(APIResponse<string>.FailResponse("Quantity must be positive"));
                }
                await _inventoryService.IncreaseStockAsync(productId, quantity);
                return Ok(APIResponse<string>.SuccessResponse("Stock decreases."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DecreaseInventory");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal server error."));
            }
        }

        [HttpPost("verify-stock")]
        public async Task<IActionResult> VerifyStock([FromBody] List<ProductStockInfoRequestDTO> requestedItems)
        {
            try
            {
                if (requestedItems == null || !requestedItems.Any())
                    return BadRequest(APIResponse<string>.FailResponse("Request list cannot be empty."));

                var results = await _inventoryService.VerifyStockForProductsAsync(requestedItems);

                return Ok(APIResponse<List<ProductStockInfoResponseDTO>>.SuccessResponse(results, "Stock verification completed successfully."));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyStock");
                return StatusCode(500, APIResponse<string>.FailResponse("An unexpected error occurred. Please try again later."));
            }
        }
    }
}
