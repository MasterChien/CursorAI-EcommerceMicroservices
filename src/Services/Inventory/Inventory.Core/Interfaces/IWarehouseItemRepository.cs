using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IWarehouseItemRepository : IRepository<WarehouseItem>
    {
        Task<IReadOnlyList<WarehouseItem>> GetByWarehouseIdAsync(int warehouseId);
        Task<IReadOnlyList<WarehouseItem>> GetByInventoryItemIdAsync(int inventoryItemId);
        Task<WarehouseItem> GetByWarehouseAndInventoryItemAsync(int warehouseId, int inventoryItemId);
        Task<bool> UpdateQuantityAsync(int warehouseItemId, int quantityChange);
        Task<bool> ReserveStockAsync(int warehouseItemId, int quantity);
        Task<bool> ReleaseReservedStockAsync(int warehouseItemId, int quantity);
    }
} 