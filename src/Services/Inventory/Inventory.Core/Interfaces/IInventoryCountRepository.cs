using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IInventoryCountRepository
    {
        Task<InventoryCount> GetByIdAsync(int id);
        Task<IEnumerable<InventoryCount>> GetAllAsync();
        Task<IEnumerable<InventoryCount>> GetByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<InventoryCount>> GetByStatusAsync(InventoryCountStatus status);
        Task<IEnumerable<InventoryCount>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<InventoryCount> AddAsync(InventoryCount inventoryCount);
        Task<InventoryCount> UpdateAsync(InventoryCount inventoryCount);
        Task<bool> DeleteAsync(int id);
    }

    public interface IInventoryCountItemRepository
    {
        Task<InventoryCountItem> GetByIdAsync(int id);
        Task<IEnumerable<InventoryCountItem>> GetByInventoryCountIdAsync(int inventoryCountId);
        Task<InventoryCountItem> GetByInventoryCountAndItemAsync(int inventoryCountId, int inventoryItemId);
        Task<InventoryCountItem> AddAsync(InventoryCountItem countItem);
        Task<InventoryCountItem> UpdateAsync(InventoryCountItem countItem);
        Task<bool> DeleteAsync(int id);
    }
} 