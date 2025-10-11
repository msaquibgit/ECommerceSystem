using OrderService.Contract.DTOs;
using OrderService.Contract.Enum;
using OrderService.Contract.ExternalServices;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.ExternalServices
{
    public class NotificationServiceClient : INotificationServiceClient
    {
        private readonly HttpClient _httpClient;

        public NotificationServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("NotificationServiceClient");
        }

        public async Task SendOrderPlacedNotificationAsync(Guid userId, Guid orderId, string accessToken)
        {
            var notification = new NotificationRequestDTO
            {
                UserId = userId,
                OrderId = orderId,
                NotificationType = NotificationTypeEnum.OrderPlaced,
                Message= "Your order has been placed successfully."
            };
            await SendNotificationAsync(notification, accessToken);
        }

        public async  Task SendOrderCancellationNotificationAsync(Guid userId, Guid orderId, string accessToken)
        {
            var notification = new NotificationRequestDTO
            {
                UserId = userId,
                OrderId = orderId,
                NotificationType = NotificationTypeEnum.OrderPlaced,
                Message = "Your order has been placed successfully."
            };
            await SendNotificationAsync(notification, accessToken);
        }



        public async Task SendOrderRefundNotificationAsync(Guid userId, Guid orderId, string accessToken)
        { 
            var notification = new NotificationRequestDTO
            {
                UserId = userId,
                OrderId = orderId,
                NotificationType = NotificationTypeEnum.RefundCompleted,
                Message = "Your refund has been processed."
            };

            await SendNotificationAsync(notification, accessToken);
        }

        public async Task SendOrderReturnNotificationAsync(Guid userId, Guid orderId, string accessToken)
        {
            var notification = new NotificationRequestDTO
            {
                UserId = userId,
                OrderId = orderId,
                NotificationType = NotificationTypeEnum.ReturnApproved,
                Message = "Your return request has been approved."
            };

            await SendNotificationAsync(notification, accessToken);
        }
        public async Task SendOrderStatusChangedNotificationAsync(Guid userId, Guid orderId, string accessToken)
        {
            var notification = new NotificationRequestDTO
            {
                UserId = userId,
                OrderId = orderId,
                NotificationType = NotificationTypeEnum.OrderStatusChanged,
                Message = "Your order status request has been changes."
            };

            await SendNotificationAsync(notification, accessToken);
        }
        private async Task SendNotificationAsync(NotificationRequestDTO notification,string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsJsonAsync("/api/notifications", notification);
            if (!response.IsSuccessStatusCode)
            {
                // Log failure or handle as needed, but do not throw here
            }
        }
    }
}
