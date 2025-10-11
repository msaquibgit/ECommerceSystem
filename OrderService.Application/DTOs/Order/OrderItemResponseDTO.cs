using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public class OrderItemResponseDTO
    {
        public Guid OrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public OrderStatusEnum ItemStatusId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;

    }
}
