using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(InventoryContext context) : base(context)
        {
        }

        public async Task<Warehouse> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.Code == code);
        }

        public async Task<IReadOnlyList<Warehouse>> GetActiveWarehousesAsync()
        {
            return await _dbSet.Where(w => w.IsActive).ToListAsync();
        }

        public async Task<Warehouse> GetWarehouseWithItemsAsync(int warehouseId)
        {
            return await _dbSet
                .Include(w => w.WarehouseItems)
                .ThenInclude(wi => wi.InventoryItem)
                .FirstOrDefaultAsync(w => w.Id == warehouseId);
        }
    }
} 