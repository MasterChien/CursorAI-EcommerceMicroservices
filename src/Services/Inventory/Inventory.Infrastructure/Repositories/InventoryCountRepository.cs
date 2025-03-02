using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Entities;
using Inventory.Core.Exceptions;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories
{
    public class InventoryCountRepository : Repository<InventoryCount>, IInventoryCountRepository
    {
        public InventoryCountRepository(InventoryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryCount>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _dbSet
                .Where(c => c.WarehouseId == warehouseId)
                .Include(c => c.Warehouse)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryCount>> GetByStatusAsync(InventoryCountStatus status)
        {
            return await _dbSet
                .Where(c => c.Status == status)
                .Include(c => c.Warehouse)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryCount>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(c => c.CountDate >= startDate && c.CountDate <= endDate)
                .Include(c => c.Warehouse)
                .ToListAsync();
        }

        public new async Task<InventoryCount> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Warehouse)
                .Include(c => c.CountItems)
                .ThenInclude(i => i.InventoryItem)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public new async Task<IEnumerable<InventoryCount>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Warehouse)
                .ToListAsync();
        }

        public new async Task<InventoryCount> UpdateAsync(InventoryCount entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class InventoryCountItemRepository : Repository<InventoryCountItem>, IInventoryCountItemRepository
    {
        public InventoryCountItemRepository(InventoryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryCountItem>> GetByInventoryCountIdAsync(int inventoryCountId)
        {
            return await _dbSet
                .Where(i => i.InventoryCountId == inventoryCountId)
                .Include(i => i.InventoryItem)
                .ToListAsync();
        }

        public async Task<InventoryCountItem> GetByInventoryCountAndItemAsync(int inventoryCountId, int inventoryItemId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(i => i.InventoryCountId == inventoryCountId && i.InventoryItemId == inventoryItemId);
        }

        public new async Task<InventoryCountItem> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(i => i.InventoryItem)
                .Include(i => i.InventoryCount)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public new async Task<InventoryCountItem> UpdateAsync(InventoryCountItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 