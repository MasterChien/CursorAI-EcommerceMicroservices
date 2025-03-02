using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IInventoryItemRepository : IRepository<InventoryItem>
    {
        Task<InventoryItem> GetByProductIdAsync(int productId);
        Task<InventoryItem> GetBySkuAsync(string sku);
        Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(int threshold);
        Task<bool> UpdateQuantityAsync(int inventoryItemId, int quantityChange);
        Task<bool> ReserveStockAsync(int inventoryItemId, int quantity);
        Task<bool> ReleaseReservedStockAsync(int inventoryItemId, int quantity);
        new Task<InventoryItem> UpdateAsync(InventoryItem entity);
    }
} 