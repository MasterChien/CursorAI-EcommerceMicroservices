using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IStockTransferRepository
    {
        Task<StockTransfer> GetByIdAsync(int id);
        Task<IEnumerable<StockTransfer>> GetAllAsync();
        Task<IEnumerable<StockTransfer>> GetBySourceWarehouseAsync(int warehouseId);
        Task<IEnumerable<StockTransfer>> GetByDestinationWarehouseAsync(int warehouseId);
        Task<IEnumerable<StockTransfer>> GetByStatusAsync(StockTransferStatus status);
        Task<IEnumerable<StockTransfer>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<StockTransfer> AddAsync(StockTransfer stockTransfer);
        Task<StockTransfer> UpdateAsync(StockTransfer stockTransfer);
        Task<bool> DeleteAsync(int id);
    }

    public interface IStockTransferItemRepository
    {
        Task<StockTransferItem> GetByIdAsync(int id);
        Task<IEnumerable<StockTransferItem>> GetByStockTransferIdAsync(int stockTransferId);
        Task<StockTransferItem> AddAsync(StockTransferItem transferItem);
        Task<StockTransferItem> UpdateAsync(StockTransferItem transferItem);
        Task<bool> DeleteAsync(int id);
    }
} 