using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.Application.DTOs.Shipments;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService ?? throw new ArgumentNullException(nameof(shipmentService));
        }

        // POST api/shipment/create
        [HttpPost("create")]
        public async Task<ActionResult<APIResponse<ShipmentResponseDTO>>> CreateShipment([FromBody] CreateShipmentRequestDTO request)
        {
            try
            {
                var shipment = await _shipmentService.CreateShipmentAsync(request);
                return Ok(APIResponse<ShipmentResponseDTO>.SuccessResposne(shipment, "Shipment created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ShipmentResponseDTO>.FailResponse("Failed to create shipment.", new List<string> { ex.Message }));
            }
        }

        // GET api/shipment/{shipmentId}
        [HttpGet("{shipmentId:guid}")]
        public async Task<ActionResult<APIResponse<ShipmentResponseDTO>>> GetShipmentById(Guid shipmentId)
        {
            try
            {
                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
                if (shipment == null)
                    return NotFound(APIResponse<ShipmentResponseDTO>.FailResponse("Shipment not found."));
                return Ok(APIResponse<ShipmentResponseDTO>.SuccessResposne(shipment,"Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ShipmentResponseDTO>.FailResponse("Failed to get shipment.", new List<string> { ex.Message }));
            }
        }

        // GET api/shipment/order/{orderId}
        [HttpGet("order/{orderId:guid}")]
        public async Task<ActionResult<APIResponse<List<ShipmentResponseDTO>>>> GetShipmentsByOrderId(Guid orderId)
        {
            try
            {
                var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(orderId);
                return Ok(APIResponse<List<ShipmentResponseDTO>>.SuccessResposne(shipments, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<List<ShipmentResponseDTO>>.FailResponse("Failed to get shipments.", new List<string> { ex.Message }));
            }
        }

        // PUT api/shipment/update-status
        [HttpPut("update-status")]
        public async Task<ActionResult<APIResponse<string>>> UpdateShipmentStatus([FromBody] ShipmentStatusUpdateRequestDTO request)
        {
            try
            {
                await _shipmentService.UpdateShipmentStatusAsync(request);
                return Ok(APIResponse<string>.SuccessResposne("Sucsess", "Shipment status updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<string>.FailResponse("Failed to update shipment status.", new List<string> { ex.Message }));
            }
        }

        // POST api/shipment/track
        [HttpPost("track")]
        public async Task<ActionResult<APIResponse<ShipmentTrackingResponseDTO>>> TrackShipment([FromBody] ShipmentTrackingRequestDTO request)
        {
            try
            {
                var trackingInfo = await _shipmentService.TrackShipmentAsync(request);
                return Ok(APIResponse<ShipmentTrackingResponseDTO>.SuccessResposne(trackingInfo, "Ok"));
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ShipmentTrackingResponseDTO>.FailResponse("Failed to track shipment.", new List<string> { ex.Message }));
            }
        }
    }
}
