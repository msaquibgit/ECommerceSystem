using Messaging.Common.Events;

namespace ProductService.Contract.Messaging
{
    public interface IOrderPlacedHandler
    {
        Task HandleAsync(OrderPlacedEvent evt);
    }
}
