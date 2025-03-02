using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IInventoryService
    {
        // Quản lý InventoryItem
        Task<InventoryItem> CreateInventoryItemAsync(InventoryItem inventoryItem);
        Task<InventoryItem> GetInventoryItemByProductIdAsync(int productId);
        Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync();
        Task<InventoryItem> UpdateInventoryItemAsync(int productId, InventoryItem inventoryItem);

        // Quản lý Warehouse
        Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
        Task<Warehouse> GetWarehouseByIdAsync(int id);
        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
        Task<Warehouse> UpdateWarehouseAsync(int id, Warehouse warehouse);
        Task<bool> DeleteWarehouseAsync(int id);

        // Quản lý số lượng trong kho
        Task<WarehouseItem> AddStockAsync(int productId, int warehouseId, int quantity);
        Task<WarehouseItem> RemoveStockAsync(int productId, int warehouseId, int quantity);
        Task<int> GetTotalStockAsync(int productId);
        Task<bool> IsInStockAsync(int productId, int quantity);

        // Quản lý đặt trước hàng
        Task<bool> ReserveStockAsync(int productId, int warehouseId, int quantity);
        Task<bool> ReleaseReservedStockAsync(int productId, int warehouseId, int quantity);
        Task<int> GetReservedStockAsync(int productId);

        // Quản lý thông tin tồn kho
        Task<IEnumerable<WarehouseItem>> GetStockByProductAsync(int productId);
        Task<IEnumerable<WarehouseItem>> GetStockByWarehouseAsync(int warehouseId);
        Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();

        // Quản lý giao dịch
        Task<IEnumerable<InventoryTransaction>> GetTransactionHistoryAsync(int productId, DateTime? startDate, DateTime? endDate);
    }
} 