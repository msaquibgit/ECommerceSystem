using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Interface
{
    public interface IInventoryService
    {
        Task<bool> IsStockAvailableAsync(Guid productId, int quantity);
        Task UpdateStockAsync(InventoryUpdateDTO inventoryUpdateDto);
        Task DecreaseStockAsync(Guid productId, int quantity);
        Task IncreaseStockAsync(Guid productId, int quantity);        
        Task<List<ProductStockInfoResponseDTO>> VerifyStockForProductsAsync(List<ProductStockInfoRequestDTO> requestedItems);

        Task IncreaseStockBulkAsync(List<InventoryUpdateDTO> stockUpdates);
        Task DecreaseStockBulkAsync(List<InventoryUpdateDTO> stockUpdates);


        // Imlement later
        //Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds);
        //Task UpdateStockAsync(Guid productId, int quantityChange);


    }
}
