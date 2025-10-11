namespace OrderService.Contract.DTOs
{
    public class UpdateStockRequestDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
