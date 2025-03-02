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
    public class InventoryItemRepository : Repository<InventoryItem>, IInventoryItemRepository
    {
        public InventoryItemRepository(InventoryContext context) : base(context)
        {
        }

        public async Task<InventoryItem> GetByProductIdAsync(int productId)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<InventoryItem> GetBySkuAsync(string sku)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.SKU == sku);
        }

        public async Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(int threshold)
        {
            return await _dbSet.Where(i => i.Quantity - i.ReservedQuantity <= threshold).ToListAsync();
        }

        public async Task<bool> UpdateQuantityAsync(int inventoryItemId, int quantityChange)
        {
            var item = await _dbSet.FindAsync(inventoryItemId);
            if (item == null)
            {
                throw new InventoryItemNotFoundException(inventoryItemId);
            }

            // Nếu là giảm số lượng, kiểm tra xem có đủ hàng không
            if (quantityChange < 0 && item.Quantity + quantityChange < item.ReservedQuantity)
            {
                throw new InsufficientStockException(
                    item.ProductId,
                    -quantityChange,
                    item.Quantity - item.ReservedQuantity);
            }

            item.Quantity += quantityChange;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReserveStockAsync(int inventoryItemId, int quantity)
        {
            var item = await _dbSet.FindAsync(inventoryItemId);
            if (item == null)
            {
                throw new InventoryItemNotFoundException(inventoryItemId);
            }

            // Kiểm tra xem có đủ hàng để đặt trước không
            if (item.AvailableQuantity < quantity)
            {
                throw new InsufficientStockException(
                    item.ProductId,
                    quantity,
                    item.AvailableQuantity);
            }

            item.ReservedQuantity += quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseReservedStockAsync(int inventoryItemId, int quantity)
        {
            var item = await _dbSet.FindAsync(inventoryItemId);
            if (item == null)
            {
                throw new InventoryItemNotFoundException(inventoryItemId);
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

        public new async Task<InventoryItem> UpdateAsync(InventoryItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
} 