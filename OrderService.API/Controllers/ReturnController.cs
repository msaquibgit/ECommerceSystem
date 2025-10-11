using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.Application.DTOs.Common;
using OrderService.Application.DTOs.Returns;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnController : ControllerBase
    {
        private readonly IReturnService _returnService;

        public ReturnController(IReturnService returnService)
        {
            _returnService = returnService ?? throw new ArgumentNullException(nameof(returnService));
        }

        [HttpPost("request")]
        public async Task<ActionResult<APIResponse<ReturnResponseDTO>>> RequestReturn([FromBody] CreateReturnRequestDTO request)
        {
            try
            {
                var result = await _returnService.CreateReturnRequestAsync(request);
                return Ok(APIResponse<ReturnResponseDTO>.SuccessResposne(result, "Return request submitted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ReturnResponseDTO>.FailResponse("Return Request failed.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{returnId:guid}")]
        public async Task<ActionResult<APIResponse<ReturnResponseDTO>>> GetReturn(Guid returnId)
        {
            try
            {
                var result = await _returnService.GetReturnByIdAsync(returnId);
                if (result == null)
                    return NotFound(APIResponse<ReturnResponseDTO>.FailResponse("Return request not found."));
                return Ok(APIResponse<ReturnResponseDTO>.SuccessResposne(result,"Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ReturnResponseDTO>.FailResponse("Failed to fetch return.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("order/{orderId:guid}")]
        public async Task<ActionResult<APIResponse<List<ReturnResponseDTO>>>> GetOrderReturns(Guid orderId)
        {
            try
            {
                var result = await _returnService.GetReturnsByOrderIdAsync(orderId);
                if (result == null)
                    return NotFound(APIResponse<ReturnResponseDTO>.FailResponse("No Return request found."));
                return Ok(APIResponse<List<ReturnResponseDTO>>.SuccessResposne(result, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<ReturnResponseDTO>>.FailResponse("Failed to fetch returns.", new List<string> { ex.Message }));
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<APIResponse<ReturnResponseDTO>>> UpdateReturn([FromBody] UpdateReturnRequestDTO request)
        {
            try
            {
                var result = await _returnService.UpdateReturnAsync(request);
                if (result == null)
                    return NotFound(APIResponse<ReturnResponseDTO>.FailResponse("Return not found or could not be updated."));
                return Ok(APIResponse<ReturnResponseDTO>.SuccessResposne(result, "Return updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ReturnResponseDTO>.FailResponse("Return update failed.", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{returnId:guid}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteReturn(Guid returnId)
        {
            try
            {
                await _returnService.DeleteReturnAsync(returnId);
                return Ok(APIResponse<string>.SuccessResposne("Deleted", "Return deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Return deletion failed.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("items/{returnId:guid}")]
        public async Task<ActionResult<APIResponse<List<ReturnItemResponseDTO>>>> GetReturnItems(Guid returnId)
        {
            try
            {
                var result = await _returnService.GetReturnItemsByReturnIdAsync(returnId);
                return Ok(APIResponse<List<ReturnItemResponseDTO>>.SuccessResposne(result,"Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<ReturnItemResponseDTO>>.FailResponse("Failed to fetch return items.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<ReturnResponseDTO>>>> GetReturnsByUser(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _returnService.GetReturnByUserAsync(userId, pageNumber, pageSize);
                return Ok(APIResponse<PaginatedResultDTO<ReturnResponseDTO>>.SuccessResposne(result, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<PaginatedResultDTO<ReturnResponseDTO>>.FailResponse("Failed to fetch returns for user.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("filter")]
        public async Task<ActionResult<APIResponse<PaginatedResultDTO<ReturnResponseDTO>>>> GetReturnsByFilter([FromBody] ReturnFilterRequestDTO filter)
        {
            try
            {
                var result = await _returnService.GetReturnAsync(filter);
                return Ok(APIResponse<PaginatedResultDTO<ReturnResponseDTO>>.SuccessResposne(result, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<PaginatedResultDTO<ReturnResponseDTO>>.FailResponse("Failed to fetch filtered returns.", new List<string> { ex.Message }));
            }
        }

        //Approve or Reject Return Request
        [HttpPut("process")]
        public async Task<ActionResult<APIResponse<ReturnApprovalResponseDTO>>> ProcessReturn([FromBody] ReturnApprovalRequestDTO request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _returnService.ApproveOrRejectReturnAsync(request, accessToken);
                return Ok(APIResponse<ReturnApprovalResponseDTO>.SuccessResposne(result, $"Return {result.Status} successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ReturnApprovalResponseDTO>.FailResponse("Return processing failed.", new List<string> { ex.Message }));
            }
        }

    }
}
