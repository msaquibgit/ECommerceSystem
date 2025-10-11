using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.DTOs
{
    public class InventoryUpdateDTO
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity Change is required.")]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "Quantity Change can be positive or negative.")]
        public int QuantityChange { get; set; }

    }
}
