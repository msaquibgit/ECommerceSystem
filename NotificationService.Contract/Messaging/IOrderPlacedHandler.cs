using Messaging.Common.Models;

namespace NotificationService.Contract.Messaging
{
    public interface IOrderPlacedHandler
    {
        Task HandleAsync(OrderPlacedEvent evt);
    }
}
