namespace Messaging.Common.Models
{
    public class EventBase
    {
        // Unique Id Per Event
       public Guid EventId { get; set; }= Guid.NewGuid();
        //When the event was created (useful for logging, debugging, or event ordering)
        public DateTime Timespan { get; set; }= DateTime.UtcNow;
        //Lets you trace a request across multiple services (e.g., Order → Payment → Notification).
        public string? CorrelationId { get; set; }
    }
}
