using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.Application.DTOs.Common;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<APIResponse<OrderResponseDTO>>> CreateOrder([FromBody] CreateOrderRequestDTO request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer", "");
                var result = await _orderService.CreateOrderAsync(request, accessToken);
                return Ok(APIResponse<OrderResponseDTO>.SuccessResposne(result, "Order placed successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Order creation failed", new List<string> { ex.Message }));
            }
        }
        [HttpPost("confirm/{orderId}")]
        public async Task<ActionResult<APIResponse<bool>>> ConfirmOrder(Guid orderId)
        {

            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer", "");
                var success = await _orderService.ConfirmOrderAsync(orderId, accessToken);
                if (!success)
                {
                    return BadRequest(APIResponse<string>.FailResponse("Failed to confirm order"));
                }
                return Ok(APIResponse<bool>.SuccessResposne(true, "Order confirm successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse(ex.Message));
            }
        }
        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<APIResponse<OrderResponseDTO>>> GetOrder(Guid orderId)
        {
            try
            {
                var result = await _orderService.GetOrderByIdAsync(orderId);
                if (result == null)
                {
                    return NotFound(APIResponse<OrderResponseDTO>.FailResponse("Order not found"));
                }
                return Ok(APIResponse<OrderResponseDTO>.SuccessResposne(result, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to fetch order", new List<string> { ex.Message }));
            }
        }
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<OrderResponseDTO>>>> GetOrderByUser(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _orderService.GetOrdersByUserAsync(userId, pageNumber, pageSize);
                if (result == null)
                {

                    return NotFound(APIResponse<PaginatedResultDTO<OrderResponseDTO>>.FailResponse("Record not found"));
                }
                return Ok(APIResponse<PaginatedResultDTO<OrderResponseDTO>>.SuccessResposne(result, "Ok"));
            }
            catch (Exception ex)
            {

                return BadRequest(APIResponse<PaginatedResultDTO<OrderResponseDTO>>.FailResponse("Record not found"));
            }
        }

        [HttpPost("filter")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<OrderResponseDTO>>>> GetOrders([FromBody] OrderFilterRequestDTO filter)
        {
            try
            {
                var result = await _orderService.GetOrdersAsync(filter);
                return Ok(APIResponse<PaginatedResultDTO<OrderResponseDTO>>.SuccessResposne(result, "Order get successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<PaginatedResultDTO<OrderResponseDTO>>.FailResponse("Failed to fetch orders.", new List<string> { ex.Message }));
            }
        }

        [HttpPut("change-status")]
        public async Task<ActionResult<APIResponse<bool>>> ChangeOrderStatus([FromBody] ChangeOrderStatusRequestDTO request)
        {
            try
            {
                var result = await _orderService.ChangeOrderStatusAsync(request);
                if (!result.Success)
                    return BadRequest(APIResponse<bool>.FailResponse("Failed to change order status."));
                return Ok(APIResponse<bool>.SuccessResposne(true, "Order status updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<OrderStatusHistoryResponseDTO>>.FailResponse("Failed to change order status", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{orderId:guid}/status-history")]
        public async Task<ActionResult<APIResponse<List<OrderStatusHistoryResponseDTO>>>> GetStatusHistory(Guid orderId)
        {
            try
            {
                var result = await _orderService.GetOrderStatusHistoryAsync(orderId);
                return Ok(APIResponse<List<OrderStatusHistoryResponseDTO>>.SuccessResposne(result, "OK"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<OrderStatusHistoryResponseDTO>>.FailResponse("Failed to fetch status history.", new List<string> { ex.Message }));
            }
        }

    }
}
