using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs.Cancellation
{
    public class UpdateCancellationItemDTO
    {
        [Required]
        public Guid Id { get; set; }  // CancellationItem Id

        [Required]
        public Guid OrderItemId { get; set; }  // Order item being cancelled

        [Range(1, int.MaxValue, ErrorMessage = "Cancelled quantity must be at least 1.")]
        public int Quantity { get; set; }
        public Guid CancellationItems {  get; set; }
    }
}
