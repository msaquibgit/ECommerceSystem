using Messaging.Common.Events;
using Messaging.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using ProductService.Contract.Messaging;

namespace ProductService.Application.Messaging
{
    public class OrderPlacedHandler : IOrderPlacedHandler
    {
        // Dependency: Inventory service used to update product stock.
        private readonly IInventoryService _inventoryService;

        // Constructor: injects IInventoryService via Dependency Injection.
        // This allows OrderPlacedHandler to call inventory logic without being tightly coupled.
        public OrderPlacedHandler(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // HandleAsync: This method is triggered whenever an OrderPlacedEvent is received from RabbitMQ.

        public async Task HandleAsync(OrderPlacedEvent evt)
        {
            // Map event items into DTOs expected by the InventoryService.
            // Each order item (product + quantity) becomes an InventoryUpdateDTO.

            var stockUpdates = evt.Items.Select(i => new InventoryUpdateDTO
            {
                ProductId=i.ProductId,
                QuantityChange=i.Quantity,
            }).ToList();

            // Call the inventory service to decrease stock for all products in bulk.
            // This ensures product quantities are reduced in the database after the order is confirmed.
            await _inventoryService.DecreaseStockBulkAsync(stockUpdates);
        }
    }
}
