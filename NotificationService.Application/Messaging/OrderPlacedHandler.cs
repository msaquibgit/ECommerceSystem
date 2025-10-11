using Messaging.Common.Events;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Contract.Messaging;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;
using System.Text.Json;

namespace NotificationService.Application.Messaging
{
    public class OrderPlacedHandler : IOrderPlacedHandler
    {
        private readonly INotificationService _notificationService;
        // injects INotificationService via DI so we can call business logic.
        public OrderPlacedHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // HandleAsync is called whenever an OrderPlacedEvent is consumed from RabbitMQ.

        public async Task HandleAsync(OrderPlacedEvent evt)
        {
            // Build structured Items array (Name, Quantity, Price)
            var items = evt.Items.Select(i => new
            {
                Name=i.ProductId.ToString(),
                Quantity=i.Quantity,
                Price=i.UnitPrice

            }).ToList();

            // Serialize items into JSON so TemplateRenderer will recognize it as JsonElement
            var itemsJson = JsonSerializer.Serialize(items);

            // Build template data dictionary (keys must match placeholders in template)
            var templateData = new Dictionary<string, object>()
            {
                {"CustomerName",evt.CustomerName },
                {"OrderNumber",evt.OrderNumber.ToString()??string.Empty },
                {"Amount",evt.TotalAmount },
                {"items",JsonDocument.Parse(itemsJson).RootElement }  // passes structured 
            };

            var request = new CreateNotificationRequestDTO
            {
                UserId=evt.UserId,
                TypeId=1, // "OrderPlaced" Type
                Channel=NotificationChannelEnum.Email,
                TemplateVersion=1,
                TemplateData= templateData,
                Recipients=new List<RecipientDTO>
                {
                    new RecipientDTO
                    {
                        RecipientTypeId=(int)RecipientTypeEnum.To,
                        Email=evt.CustomerEmail,
                        PhoneNumber=evt.PhoneNumber,
                    }
                },
                Priority=NotificationPriorityEnum.High,
                ScheduledAtUtc=null,
                CreatedBy="OrderPlacedHandler"
            };

            // Persist notification
            await _notificationService.CreateAsync(request);

        }
    }
}
