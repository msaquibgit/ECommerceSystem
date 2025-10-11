using Messaging.Common.Events;

namespace NotificationService.Contract.Messaging
{
    public interface IOrderPlacedHandler 
    {
        Task HandleAsync(OrderPlacedEvent evt);
    }
}
