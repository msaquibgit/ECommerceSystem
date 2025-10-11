using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.Application.DTOs.Common;
using OrderService.Application.DTOs.Refunds;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;
        public RefundController(IRefundService refundService)
        {
            _refundService = refundService ?? throw new ArgumentNullException(nameof(refundService));
        }

        // GET api/refund/{refundId}
        [HttpGet("{refundId:guid}")]
        public async Task<ActionResult<APIResponse<RefundResponseDTO>>> GetRefundById(Guid refundId)
        {
            try
            {
                var refund = await _refundService.GetRefundByIdAsync(refundId);
                if (refund == null)
                    return NotFound(APIResponse<RefundResponseDTO>.FailResponse("Refund not found."));
                return Ok(APIResponse<RefundResponseDTO>.SuccessResposne(refund, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<RefundResponseDTO>.FailResponse("Failed to get refund.", new List<string> { ex.Message }));
            }
        }

        // DELETE api/refund/{refundId}
        [HttpDelete("{refundId:guid}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteRefund(Guid refundId)
        {
            try
            {
                await _refundService.DeleteRefundAsync(refundId);
                return Ok(APIResponse<string>.SuccessResposne("Success", "Refund deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to delete refund.", new List<string> { ex.Message }));
            }
        }

        // GET api/refund/cancellation/{cancellationId}
        [HttpGet("cancellation/{cancellationId:guid}")]
        public async Task<ActionResult<APIResponse<RefundResponseDTO>>> GetRefundByCancellationId(Guid cancellationId)
        {
            try
            {
                var refund = await _refundService.GetByCancellationIdAsync(cancellationId);
                if (refund == null)
                    return NotFound(APIResponse<RefundResponseDTO>.FailResponse("Refund not found for given cancellation."));
                return Ok(APIResponse<RefundResponseDTO>.SuccessResposne(refund, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<RefundResponseDTO>.FailResponse("Failed to get refund by cancellation.", new List<string> { ex.Message }));
            }
        }

        // GET api/refund/return/{returnId}
        [HttpGet("return/{returnId:guid}")]
        public async Task<ActionResult<APIResponse<RefundResponseDTO>>> GetRefundByReturnId(Guid returnId)
        {
            try
            {
                var refund = await _refundService.GetByReturnIdAsync(returnId);
                if (refund == null)
                    return NotFound(APIResponse<RefundResponseDTO>.FailResponse("Refund not found for given return."));
                return Ok(APIResponse<RefundResponseDTO>.SuccessResposne(refund, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<RefundResponseDTO>.FailResponse("Failed to get refund by return.", new List<string> { ex.Message }));
            }
        }

        // GET api/refund/order/{orderId}
        [HttpGet("order/{orderId:guid}")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<RefundResponseDTO>>>> GetRefundsByOrderId(
            Guid orderId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var refunds = await _refundService.GetRefundsByOrderIdAsync(orderId, pageNumber, pageSize);
                return Ok(APIResponse<PaginatedResultDTO<RefundResponseDTO>>.SuccessResposne(refunds, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<PaginatedResultDTO<RefundResponseDTO>>.FailResponse("Failed to get refunds by order.", new List<string> { ex.Message }));
            }
        }

        // POST api/refund/filter
        [HttpPost("filter")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<RefundResponseDTO>>>> GetRefundsByFilter([FromBody] RefundFilterRequestDTO filter)
        {
            try
            {
                var refunds = await _refundService.GetRefundsAsync(filter);
                return Ok(APIResponse<PaginatedResultDTO<RefundResponseDTO>>.SuccessResposne(refunds, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<PaginatedResultDTO<RefundResponseDTO>>.FailResponse("Failed to get refunds with filter.", new List<string> { ex.Message }));
            }
        }

        // PUT api/refund/update-status
        [HttpPut("update-status")]
        public async Task<ActionResult<APIResponse<UpdateRefundStatusResponseDTO>>> UpdateRefundStatus(UpdateRefundStatusRequestDTO request)
        {
            try
            {
                var result = await _refundService.UpdateRefundStatusAsync(request);
                return Ok(APIResponse<UpdateRefundStatusResponseDTO>.SuccessResposne(result, $"Refund {result.Status} successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to update refund status.", new List<string> { ex.Message }));
            }
        }

    }
}
