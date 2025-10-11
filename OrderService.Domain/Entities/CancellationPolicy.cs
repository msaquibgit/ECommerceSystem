using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.Entities
{
    public class CancellationPolicy
    {
        [Key]
        public int Id { get; set; }

        //Standard Cancellation Policy, No Cancellation After Shipment

        [Required, MaxLength(150)]
        public string PolicyName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int AllowedCancellationDays { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal PenaltyPercentage { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
}
