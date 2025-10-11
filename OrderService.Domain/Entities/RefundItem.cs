using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain.Entities
{
    public class RefundItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RefundId { get; set; }
        public Refund? Refund { get; set; }

        public Guid OrderItemId { get; set; }
        public OrderItem? OrderItem { get; set; }

        public int RefundedQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundedAmount { get; set; }

    }
}
