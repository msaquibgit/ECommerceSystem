using NotificationService.Application.DTOs;
using NotificationService.Application.Utilities;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;

namespace NotificationService.Application.Interfaces
{
    public interface INotificationChannelHandler
    {
        NotificationChannelEnum Channel { get; }
        Task<(bool Success, string? ProviderMessage, string? Error)> SendAsync(Notification notification);

        

    }
}
