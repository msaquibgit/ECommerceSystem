using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTOs;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;

namespace ProductService.API.Controllers
{
    [Route("api/discounts")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly ILogger<DiscountController> _logger;

        public DiscountController(IDiscountService discountService, ILogger<DiscountController> logger)
        {
            _discountService = discountService;
            _logger = logger;
        }

        [HttpGet("by-product/{productId:guid}")]
        [ProducesResponseType(typeof(APIResponse<List<DiscountDTO>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>),StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            try
            {
                var discount = await _discountService.GetByProductIdAsync(productId);
                if(discount == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Not found"));
                }
                return Ok(APIResponse<List<DiscountDTO>>.SuccessResponse(discount, "Product get successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Get product discount by id");
                return StatusCode(500,APIResponse<string>.FailResponse("Internal Server Error"));
            }

        }
        [HttpGet("active/{productid:guid}")]
        [ProducesResponseType(typeof(APIResponse<DiscountDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetActiveDiscountByProdctId(Guid productid)
        {
            try
            {
                var activeDiscount = await _discountService.GetActiveDiscountByProductIdAsync(productid);
                if (activeDiscount == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Not found"));
                }
                return Ok(APIResponse<DiscountDTO>.SuccessResponse(activeDiscount, "Product get successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Get product discount by id");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error"));
            }
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(APIResponse<DiscountDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([Required][FromBody] DiscountCreateDTO dto)
        {
            try
            {
                var discount = await _discountService.AddAsync(dto);
                if (discount == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Not found"));
                }
                return Ok(APIResponse<DiscountDTO>.SuccessResponse(discount, "Product get successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Get product discount by id");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error"));
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<DiscountDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([Required][FromBody] DiscountUpdateDTO dto)
        {
            try
            {
                var discount = await _discountService.UpdateAsync(dto);
                if (discount == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Not found"));
                }
                return Ok(APIResponse<DiscountDTO>.SuccessResponse(discount, "Product get successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Get product discount by id");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error"));
            }
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
               await _discountService.DeleteAsync(id);
               
                    return Ok(APIResponse<string>.SuccessResponse(string.Empty, "Category deleted successfully"));                               
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in Get All");
                return StatusCode(500, APIResponse<string>.FailResponse("Internal Server Error."));
            }
        }
    }
}
