namespace Messaging.Common.Options
{
    public class RabbitMqOptions
    {
        // connection
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 15672;
        public string UserName { get; set; } = "ecommerce_user";
        public string Password { get; set; } = "Test@1234";
        public string VirtualHost { get; set; } = "ecommerce_vhost";

        //Exchange(s)
        public string ExchangeName { get; set; } = "ecommerce_user";

        // Dead-lettering (optional but recommended)
        public string? DlxExchangeName { get; set; } = "ecommerce.dlx";
        public string? DlxQueueName { get; set; } = "ecommerce.dlq";

        // Queues we care about for this feature set
        public string ProductOrderPlacedQueue { get; set; } = "product.order_placed";
        public string NotificationOrderPlacedQueue { get; set; } = "notification.order_placed";


    }
}
