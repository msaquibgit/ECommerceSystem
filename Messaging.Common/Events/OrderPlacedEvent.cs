namespace Messaging.Common.Models
{
    //Raised by Order Microservice when a new order is placed
    //Consumed by Orchestrator Microservice, then Orchestrator service Raise StockReservationRequestedEvent
    public sealed class OrderPlacedEvent : EventBase
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<OrderItemLine> Items { get; set; } = new();
    }


}
