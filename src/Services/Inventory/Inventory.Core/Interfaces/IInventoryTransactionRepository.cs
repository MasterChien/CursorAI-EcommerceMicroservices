using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces
{
    public interface IInventoryTransactionRepository : IRepository<InventoryTransaction>
    {
        Task<IReadOnlyList<InventoryTransaction>> GetByInventoryItemIdAsync(int inventoryItemId);
        Task<IReadOnlyList<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IReadOnlyList<InventoryTransaction>> GetByTransactionTypeAsync(TransactionType transactionType);
        Task<IReadOnlyList<InventoryTransaction>> GetByReferenceNumberAsync(string referenceNumber);
    }
} 