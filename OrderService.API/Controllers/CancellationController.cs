using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.Application.DTOs.Cancellation;
using OrderService.Application.DTOs.Common;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly ICancellationService _cancellationService;

        public CancellationController(ICancellationService cancellationService)
        {
            _cancellationService = cancellationService ?? throw new ArgumentNullException(nameof(cancellationService));
        }

        [HttpPost("request")]
        public async Task<ActionResult<APIResponse<CancellationResponseDTO>>> RequestCancellation([FromBody] CreateCancellationRequestDTO request)
        {
            try
            {
                var result = await _cancellationService.CreateCancellationRequestAsync(request);
                
                return Ok(APIResponse<CancellationResponseDTO>.SuccessResposne(result, "Cancellation request submitted successfully."));
            }
            catch (Exception ex)
            {

                return BadRequest(APIResponse<CancellationResponseDTO>.FailResponse("Cancellation Failed", new List<string> { ex.Message }));
            }
        }
        [HttpGet("{cancellationId:guid}")]
        public async Task<ActionResult<APIResponse<CancellationResponseDTO>>> GetCancellationById(Guid cancellationId)
        {
            try
            {
                var result = await _cancellationService.GetCancellationByIdAsync(cancellationId);
                if (result == null)
                    return NotFound(APIResponse<CancellationResponseDTO>.FailResponse("Cancellation request not found."));
                return Ok(APIResponse<CancellationResponseDTO>.SuccessResposne(result, "Sucess"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<CancellationResponseDTO>.FailResponse("Failed to fetch cancellation.", new List<string> { ex.Message }));
            }
        }
        [HttpGet("order/{orderId:guid}")]
        public async Task<ActionResult<APIResponse<List<CancellationResponseDTO>>>> GetCancellationsByOrderId(Guid orderId)
        {
            try
            {
                var result = await _cancellationService.GetCancellationsByOrderIdAsync(orderId);
                if (result == null)
                    return NotFound(APIResponse<List<CancellationResponseDTO>>.FailResponse("No Cancellation request found for this order."));
                return Ok(APIResponse<List<CancellationResponseDTO>>.SuccessResposne(result,"Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<CancellationResponseDTO>>.FailResponse("Failed to fetch cancellations.", new List<string> { ex.Message }));
            }
        }
        [HttpPut("update")]
        public async Task<ActionResult<APIResponse<CancellationResponseDTO>>> UpdateCancellation([FromBody] UpdateCancellationRequestDTO request)
        {
            try
            {
                var result = await _cancellationService.UpdateCancellationAsync(request);
                if (result == null)
                    return NotFound(APIResponse<CancellationResponseDTO>.FailResponse("Cancellation not found or could not be updated."));
                return Ok(APIResponse<CancellationResponseDTO>.SuccessResposne(result, "Cancellation updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<CancellationResponseDTO>.FailResponse("Cancellation update failed.", new List<string> { ex.Message }));
            }
        }
        [HttpDelete("{cancellationId:guid}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteCancellation(Guid cancellationId)
        {
            try
            {
                await _cancellationService.DeleteCancellationAsync(cancellationId);
                return Ok(APIResponse<string>.SuccessResposne("Sucess", "Cancellation deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Cancellation deletion failed.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("items/{cancellationId:guid}")]
        public async Task<ActionResult<APIResponse<List<CancellationItemResponseDTO>>>> GetCancellationItemsByCancellationId(Guid cancellationId)
        {
            try
            {
                var result = await _cancellationService.GetCancellationItemsByCancellationIdAsync(cancellationId);
                return Ok(APIResponse<List<CancellationItemResponseDTO>>.SuccessResposne(result,"Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<CancellationItemResponseDTO>>.FailResponse("Failed to fetch cancellation items.", new List<string> { ex.Message }));
            }
        }
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<CancellationResponseDTO>>>> GetCancellationsByUserId(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _cancellationService.GetCancellationsByUserAsync(userId, pageNumber, pageSize);
                return Ok(APIResponse<PaginatedResultDTO<CancellationResponseDTO>>.SuccessResposne(result,"Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<PaginatedResultDTO<CancellationResponseDTO>>.FailResponse("Failed to fetch cancellations for user.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("filter")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<CancellationResponseDTO>>>> GetCancellationsByFilter([FromBody] CancellationFilterRequestDTO filter)
        {
            try
            {
                var result = await _cancellationService.GetCancellationsAsync(filter);
                return Ok(APIResponse<PaginatedResultDTO<CancellationResponseDTO>>.SuccessResposne(result,"Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<PaginatedResultDTO<CancellationResponseDTO>>.FailResponse("Failed to fetch filtered cancellations.", new List<string> { ex.Message }));
            }
        }

        [HttpPut("process")]
        public async Task<ActionResult<APIResponse<CancellationApprovalResponseDTO>>> ProcessCancellation([FromBody] CancellationApprovalRequestDTO request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _cancellationService.ApproveOrRejectCancellationAsync(request, accessToken);
                return Ok(APIResponse<CancellationApprovalResponseDTO>.SuccessResposne(result, $"Cancellation {result.Status} successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<CancellationApprovalResponseDTO>.FailResponse("Cancellation processing failed.", new List<string> { ex.Message }));
            }
        }

    }
}
