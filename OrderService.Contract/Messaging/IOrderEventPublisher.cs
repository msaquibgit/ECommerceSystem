using Messaging.Common.Models;

namespace OrderService.Contract.Messaging
{
    public interface IOrderEventPublisher
    {
        Task PublishOrderPlacedAsync(OrderPlacedEvent evt, string? correlationId = null);
    }
}
