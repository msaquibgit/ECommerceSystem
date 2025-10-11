using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationService.API.DTOs;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;

namespace NotificationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IPreferenceService _preferenceService;

        public NotificationController(
            INotificationService notificationService,
            IPreferenceService preferenceService)
        {
            _notificationService = notificationService;
            _preferenceService = preferenceService;
        }

        // === Create Notification ===
        [HttpPost("create")]
        public async Task<ActionResult<APIResponse<Guid>>> Create([FromBody] CreateNotificationRequestDTO dto)
        {
            try
            {
                var id = await _notificationService.CreateAsync(dto);
                return Ok(APIResponse<Guid>.SuccessResponse(id, "Notification created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<Guid>.FailResponse("Failed to create notification", new List<string> { ex.Message }));
            }
        }

        // === Get Notifications by User ===
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<APIResponse<List<NotificationResponseDTO>>>> GetByUser(Guid userId, int take = 50, int skip = 0)
        {
            try
            {
                var data = await _notificationService.GetUserAsync(userId, take, skip);
                return Ok(APIResponse<List<NotificationResponseDTO>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<NotificationResponseDTO>>.FailResponse("Failed to fetch notifications", new List<string> { ex.Message }));
            }
        }

        // === Process Queue Batch ===
        [HttpPost("process-queue")]
        public async Task<ActionResult<APIResponse<string>>> ProcessQueue(int take = 10, int skip = 0)
        {
            try
            {
                await _notificationService.ProcessQueueBatchAsync(take, skip);
                return Ok(APIResponse<string>.SuccessResponse("Queue processed successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to process queue", new List<string> { ex.Message }));
            }
        }

        // === Disable Notification ===
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<APIResponse<string>>> Disable(Guid id)
        {
            try
            {
                await _notificationService.DisableAsync(id);
                return Ok(APIResponse<string>.SuccessResponse("Notification disabled successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to disable notification", new List<string> { ex.Message }));
            }
        }

        // === Manage Preferences ===
        [HttpPost("preferences")]
        public async Task<ActionResult<APIResponse<string>>> UpsertPreferences([FromBody] PreferenceDTO dto)
        {
            try
            {
                await _preferenceService.UpsertAsync(dto);
                return Ok(APIResponse<string>.SuccessResponse("Preferences updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to update preferences", new List<string> { ex.Message }));
            }
        }

    }
}
