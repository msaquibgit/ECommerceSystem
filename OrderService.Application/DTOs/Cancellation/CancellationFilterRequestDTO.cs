using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Cancellation
{
    public class CancellationFilterRequestDTO
    {
        public Guid? UserId { get; set; }
        public CancellationStatusEnum? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

    }
}
