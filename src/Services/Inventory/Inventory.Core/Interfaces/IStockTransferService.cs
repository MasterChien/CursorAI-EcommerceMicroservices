using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IStockTransferService
    {
        // Quản lý phiên chuyển kho
        Task<StockTransfer> CreateStockTransferAsync(StockTransfer stockTransfer, IEnumerable<StockTransferItem> transferItems);
        Task<StockTransfer> GetStockTransferByIdAsync(int id);
        Task<IEnumerable<StockTransfer>> GetStockTransfersByWarehouseAsync(int warehouseId, bool isSource);
        Task<IEnumerable<StockTransfer>> GetStockTransfersByStatusAsync(StockTransferStatus status);
        Task<IEnumerable<StockTransfer>> GetAllStockTransfersAsync();
        
        // Quản lý trạng thái chuyển kho
        Task<StockTransfer> UpdateStockTransferStatusAsync(int id, StockTransferStatus status, string approvedBy, string notes);
        Task<StockTransfer> ApproveStockTransferAsync(int id, string approvedBy, string notes);
        Task<StockTransfer> CompleteStockTransferAsync(int id, string notes);
        Task<StockTransfer> CancelStockTransferAsync(int id, string reason);
        
        // Quản lý các mục chuyển kho
        Task<StockTransferItem> UpdateStockTransferItemAsync(int transferId, int itemId, int quantity, string notes);
        Task<IEnumerable<StockTransferItem>> GetStockTransferItemsAsync(int transferId);
        
        // Quản lý mặt hàng
        Task<InventoryItem> GetInventoryItemByIdAsync(int id);
        
        // Quản lý kho
        Task<Warehouse> GetWarehouseByIdAsync(int id);
    }
} 