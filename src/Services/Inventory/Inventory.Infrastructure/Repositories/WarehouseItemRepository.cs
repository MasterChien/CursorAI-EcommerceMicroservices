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
    public class WarehouseItemRepository : Repository<WarehouseItem>, IWarehouseItemRepository
    {
        public WarehouseItemRepository(InventoryContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<WarehouseItem>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _dbSet
                .Include(wi => wi.InventoryItem)
                .Where(wi => wi.WarehouseId == warehouseId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<WarehouseItem>> GetByInventoryItemIdAsync(int inventoryItemId)
        {
            return await _dbSet
                .Include(wi => wi.Warehouse)
                .Where(wi => wi.InventoryItemId == inventoryItemId)
                .ToListAsync();
        }

        public async Task<WarehouseItem> GetByWarehouseAndInventoryItemAsync(int warehouseId, int inventoryItemId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && wi.InventoryItemId == inventoryItemId);
        }

        public async Task<bool> UpdateQuantityAsync(int warehouseItemId, int quantityChange)
        {
            var item = await _dbSet.FindAsync(warehouseItemId);
            if (item == null)
            {
                throw new InventoryItemNotFoundException(warehouseItemId);
            }

            // Nếu là giảm số lượng, kiểm tra xem có đủ hàng không
            if (quantityChange < 0 && item.Quantity + quantityChange < item.ReservedQuantity)
            {
                throw new InsufficientStockException(
                    0, // Không có ProductId ở đây
                    -quantityChange,
                    item.Quantity - item.ReservedQuantity);
            }

            item.Quantity += quantityChange;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReserveStockAsync(int warehouseItemId, int quantity)
        {
            var item = await _dbSet.FindAsync(warehouseItemId);
            if (item == null)
            {
                throw new InventoryItemNotFoundException(warehouseItemId);
            }

            // Kiểm tra xem có đủ hàng để đặt trước không
            if (item.AvailableQuantity < quantity)
            {
                throw new InsufficientStockException(
                    0, // Không có ProductId ở đây
                    quantity,
                    item.AvailableQuantity);
            }

            item.ReservedQuantity += quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseReservedStockAsync(int warehouseItemId, int quantity)
        {
            var item = await _dbSet.FindAsync(warehouseItemId);
            if (item == null)
            {
                throw new InventoryItemNotFoundException(warehouseItemId);
            }

            // Kiểm tra xem số lượng giải phóng có hợp lệ không
            if (quantity > item.ReservedQuantity)
            {
                quantity = item.ReservedQuantity; // Giải phóng tối đa số lượng đã đặt trước
            }

            item.ReservedQuantity -= quantity;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 