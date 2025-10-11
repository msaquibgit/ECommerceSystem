using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Refunds
{
    public class RefundFilterRequestDTO 
    {
        public Guid? UserId { get; set; }
        public RefundStatusEnum? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

    }
}
