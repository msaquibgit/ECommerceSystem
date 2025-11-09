using Messaging.Common.Models;

namespace ProductService.Contract.Messaging
{
    public interface IOrderPlacedHandler
    {
        Task HandleAsync(OrderPlacedEvent evt);
    }
}
