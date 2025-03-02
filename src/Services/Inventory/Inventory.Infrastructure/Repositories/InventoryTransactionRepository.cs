using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories
{
    public class InventoryTransactionRepository : Repository<InventoryTransaction>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(InventoryContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<InventoryTransaction>> GetByInventoryItemIdAsync(int inventoryItemId)
        {
            return await _dbSet
                .Where(t => t.InventoryItemId == inventoryItemId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<InventoryTransaction>> GetByTransactionTypeAsync(TransactionType transactionType)
        {
            return await _dbSet
                .Where(t => t.TransactionType == transactionType)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<InventoryTransaction>> GetByReferenceNumberAsync(string referenceNumber)
        {
            return await _dbSet
                .Where(t => t.ReferenceNumber == referenceNumber)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
} 