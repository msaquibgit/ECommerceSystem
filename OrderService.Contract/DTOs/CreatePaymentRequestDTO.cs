using OrderService.Contract.Enum;

namespace OrderService.Contract.DTOs
{
    public class CreatePaymentRequestDTO
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }

    }
}
