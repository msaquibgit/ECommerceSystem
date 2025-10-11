using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.Application.DTOs.Cart;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService?? throw new ArgumentNullException(nameof(cartService));
        }
        [HttpPost("add-item")]
        public async Task<ActionResult<APIResponse<CartItemResponseDTO>>> AddItem([FromBody] AddCartItemRequestDTO request)
        {
            try
            {
                var result = await _cartService.AddItemToCartAsync(request);
                if (result == null)
                {
                    return NotFound(APIResponse<CartResponseDTO>.FailResponse("Cart Not Found"));
                }
                return Ok(APIResponse<CartResponseDTO>.SuccessResposne(result,"Item added to cart successfully."));
            }
            catch(Exception ex)
            {
                return BadRequest(APIResponse<CartResponseDTO>.FailResponse("Fail to add item",new List<string> { ex.Message}));
            }
        }
        [HttpPut("update-item")]
        public async Task<ActionResult<APIResponse<CartResponseDTO>>> UpdateItem([FromBody] UpdateCartItemRequestDTO request)
        {
            try
            {
                var result =await  _cartService.UpdateCartItemAsync(request);
                if(result == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Cart not found."));
                }
                return Ok(APIResponse<CartResponseDTO>.SuccessResposne(result, "Cart item updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Fail to update item",new List<string> { ex.Message}));
            }
        }
        [HttpDelete("rremove-item")]
        public async Task<ActionResult<APIResponse<CartResponseDTO>>> RemoveItem([FromBody] RemoveCartItemRequestDTO request)
        {
            try
            {
                var result = await _cartService.RemoveCartItemAsync(request);
                if (result == null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Cart not found"));
                }
                return Ok(APIResponse<CartResponseDTO>.SuccessResposne(result, "Cart deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Fail to remove item", new List<string> { ex.Message }));
            }
        }
        [HttpDelete("clear")]
        public async Task<ActionResult<APIResponse<string>>> ClearCart([FromBody] ClearCartRequestDTO request)
        {
            try
            {
                await _cartService.ClearCartAsync(request);
                return Ok(APIResponse<string>.SuccessResposne("Cart cleared", "Cart cleared successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to clear cart", new List<string> { ex.Message }));
            }
        }
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<APIResponse<CartResponseDTO>>> GetCartItem(Guid userId)
        {
            try
            {
                var result = await _cartService.GetCartItemsAsync(userId);
                if(result==null)
                {
                    return NotFound(APIResponse<string>.FailResponse("Cart not found"));
                }
                return Ok(APIResponse<CartResponseDTO>.SuccessResposne(result, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to fetch cart",new List<string> { ex.Message}));
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse<CartResponseDTO>>> MergeCarts([FromQuery] Guid targrtUserdId, [FromQuery] Guid souceUserId)
        {
            try
            {
                var result = await _cartService.MergeCartsAsync(targrtUserdId, souceUserId);
                if(result == null)
                {
                   return  NotFound(APIResponse<string>.FailResponse("Cart not found"));
                }
                return Ok(APIResponse<CartResponseDTO>.SuccessResposne(result, "Cart Meged successgill."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to merge cart", new List<string> { ex.Message }));
            }
        }
    }
}
