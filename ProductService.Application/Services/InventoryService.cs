using ProductService.Application.DTOs;
using ProductService.Application.Interface;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Services
{
    public class InventoryService:IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsStockAvailableAsync(Guid productId, int quantity)
        {
            return await _repository.IsStockAvailableAsync(productId, quantity);
        }

        public async Task UpdateStockAsync(InventoryUpdateDTO inventoryUpdateDto)
        {
            await _repository.UpdateStockAsync(inventoryUpdateDto.ProductId, inventoryUpdateDto.QuantityChange);
        }

        //public async Task DecreaseStockBulkAsync(List<InventoryUpdateDTO> inventoryUpdateDto)
        //{
        //    foreach(var i in inventoryUpdateDto)
        //    {
        //        await _repository.UpdateStockBulkAsync(i.ProductId,i.QuantityChange);
        //    }
            
        //}


        public async Task DecreaseStockAsync(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            // Check if enough stock is available
            bool available = await IsStockAvailableAsync(productId, quantity);
            if (!available)
                throw new InvalidOperationException("Insufficient stock to decrease.");

            await _repository.IncreaseDecsreaseStockAsync(productId, -quantity);
        }

        public async Task IncreaseStockAsync(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            await _repository.IncreaseDecsreaseStockAsync(productId, quantity);
        }

        public async Task<List<ProductStockInfoResponseDTO>> VerifyStockForProductsAsync(List<ProductStockInfoRequestDTO> requestedItems)
        {
            var productIds = requestedItems.Select(x => x.ProductId).Distinct();
            var products = await _repository.GetProductsByIdsAsync(productIds);
            var results = new List<ProductStockInfoResponseDTO>();
            foreach (var item in requestedItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                {
                    results.Add(new ProductStockInfoResponseDTO
                    {
                        ProductId = item.ProductId,
                        IsValidProduct = false,
                        IsQuantityAvailable = false,
                        AvailableQuantity = 0
                    });
                }
                else
                {
                    bool isAvailable = product.StockQuantity >= item.Quantity;
                    results.Add(new ProductStockInfoResponseDTO
                    {
                        ProductId = item.ProductId,
                        IsValidProduct = true,
                        IsQuantityAvailable = isAvailable,
                        AvailableQuantity = product.StockQuantity
                    });
                }
            }
            return results;
        }

        public async Task IncreaseStockBulkAsync(List<InventoryUpdateDTO> stockUpdates)
        {
            var updates = stockUpdates.Select(s => (s.ProductId, s.QuantityChange));
            await _repository.IncreaseStockBulkAsync(updates);
        }

        public async Task DecreaseStockBulkAsync(List<InventoryUpdateDTO> stockUpdates)
        {
            var updates = stockUpdates.Select(s => (s.ProductId, s.QuantityChange));
            await _repository.DecreaseStockBulkAsync(updates);
        }

    }


}

