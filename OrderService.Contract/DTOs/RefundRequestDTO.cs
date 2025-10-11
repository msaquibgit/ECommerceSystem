namespace OrderService.Contract.DTOs
{
    public class RefundRequestDTO
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid? CancellationId { get; set; }
        public Guid? ReturnId { get; set; }
        public decimal? RefundAmount { get; set; }
        public string Reason { get; set; } = null!;

    }
}
