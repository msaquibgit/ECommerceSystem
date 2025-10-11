using Messaging.Common.Events;

namespace OrderService.Contract.Messaging
{
    public interface IOrderEventPublisher
    {
        Task PublishOrderPlacedAsync(OrderPlacedEvent evt, string? correlationId = null);
    }
}
