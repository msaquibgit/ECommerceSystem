namespace OrderService.Contract.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountedPrice { get; set; }

    }
}
