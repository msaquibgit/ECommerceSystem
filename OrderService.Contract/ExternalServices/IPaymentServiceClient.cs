using OrderService.Contract.DTOs;

namespace OrderService.Contract.ExternalServices
{
    public interface IPaymentServiceClient
    {
        Task<CreatePaymentResponseDTO> InitiatePaymentAsync(CreatePaymentRequestDTO request, string accessToken);
        Task<PaymentInfoResponseDTO?> GetPaymentInfoAsync(PaymentInfoRequestDTO request, string accessToken);
        Task<RefundResponseDTO> InitiateRefundAsync(RefundRequestDTO request, string accessToken);

    }
}
