using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.Entities
{
    public class CancellationItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CancellationId { get; set; }
        public Cancellation Cancellation { get; set; } = null!;

        public Guid OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchaseAmount { get; set; }
        public decimal? CancellationCharge { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }


    }
}