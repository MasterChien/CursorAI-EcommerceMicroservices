using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IInventoryCountService
    {
        // Quản lý phiên kiểm kê
        Task<InventoryCount> CreateInventoryCountAsync(int warehouseId, string countBy, string notes);
        Task<InventoryCount> GetInventoryCountByIdAsync(int id);
        Task<IEnumerable<InventoryCount>> GetInventoryCountsByWarehouseAsync(int warehouseId);
        Task<IEnumerable<InventoryCount>> GetInventoryCountsByStatusAsync(InventoryCountStatus status);
        Task<IEnumerable<InventoryCount>> GetAllInventoryCountsAsync();
        
        // Quản lý các mục kiểm kê
        Task<InventoryCountItem> UpdateInventoryCountItemAsync(int countId, int inventoryItemId, int actualQuantity, string notes);
        Task<IEnumerable<InventoryCountItem>> GetInventoryCountItemsAsync(int countId);
        
        // Quản lý quy trình kiểm kê
        Task<InventoryCount> StartInventoryCountAsync(int countId);
        Task<InventoryCount> CompleteInventoryCountAsync(int countId, IEnumerable<InventoryCountItem> countItems, string notes);
        Task<InventoryCount> CancelInventoryCountAsync(int countId, string reason);
        
        // Báo cáo và phân tích
        Task<Dictionary<string, int>> GetInventoryDiscrepancySummaryAsync(int countId);
    }
} 