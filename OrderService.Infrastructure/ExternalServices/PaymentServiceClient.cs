using OrderService.Contract.DTOs;
using OrderService.Contract.ExternalServices;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.ExternalServices
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;

        public PaymentServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PaymentServiceClient");
        }


        public async Task<CreatePaymentResponseDTO> InitiatePaymentAsync(CreatePaymentRequestDTO request, string accessToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/payments/create")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CreatePaymentResponseDTO>>();
            return apiResponse?.Data!;
        }

        public async Task<PaymentInfoResponseDTO?> GetPaymentInfoAsync(PaymentInfoRequestDTO request, string accessToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/payments/info")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentInfoResponseDTO>>();
            return apiResponse?.Success == true ? apiResponse.Data : null;
        }

        public async Task<RefundResponseDTO> InitiateRefundAsync(RefundRequestDTO request, string accessToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/payments/refund")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<RefundResponseDTO>>();
            return apiResponse?.Data!;
        }
    }
}
