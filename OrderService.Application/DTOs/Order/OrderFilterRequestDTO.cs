using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public class OrderFilterRequestDTO
    {
        public Guid? UserId { get; set; }
        public OrderStatusEnum? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; } // order number, email etc.
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 

    }
}
