using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Cancellation
{
    public class CancellationResponseDTO
    {
        public Guid CancellationId { get; set; }
        public Guid OrderId { get; set; }
        public Guid CancellationItems { get; set; }
        public Guid UserId { get; set; }
        public CancellationStatusEnum Status { get; set; }
        public int ReasonId { get; set; }
        public string ReasonText { get; set; } = null!;
        public string? Remarks { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public List<CancellationItemResponseDTO> Items { get; set; } = new List<CancellationItemResponseDTO>();

    }
}
