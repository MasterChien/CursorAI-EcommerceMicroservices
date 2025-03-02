using Inventory.Core.Entities;
using Inventory.Core.Exceptions;
using Inventory.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseItemRepository _warehouseItemRepository;
        private readonly IInventoryTransactionRepository _transactionRepository;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IInventoryItemRepository inventoryItemRepository,
            IWarehouseRepository warehouseRepository,
            IWarehouseItemRepository warehouseItemRepository,
            IInventoryTransactionRepository transactionRepository,
            ILogger<InventoryService> logger)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseItemRepository = warehouseItemRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<InventoryItem> CreateInventoryItemAsync(InventoryItem inventoryItem)
        {
            _logger.LogInformation("Tạo mục kho mới: {ProductId}, {ProductName}", inventoryItem.ProductId, inventoryItem.ProductName);
            
            var existingItem = await _inventoryItemRepository.GetByProductIdAsync(inventoryItem.ProductId);
            if (existingItem != null)
            {
                throw new InventoryException($"Mục kho với ProductId {inventoryItem.ProductId} đã tồn tại");
            }

            return await _inventoryItemRepository.AddAsync(inventoryItem);
        }

        public async Task<InventoryItem> GetInventoryItemByProductIdAsync(int productId)
        {
            _logger.LogInformation("Lấy thông tin mục kho theo ProductId: {ProductId}", productId);
            return await _inventoryItemRepository.GetByProductIdAsync(productId);
        }

        public async Task<IEnumerable<InventoryItem>> GetAllInventoryItemsAsync()
        {
            _logger.LogInformation("Lấy tất cả các mục kho");
            var items = await _inventoryItemRepository.GetAllAsync();
            return items;
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            _logger.LogInformation("Tạo kho mới: {Name}", warehouse.Name);
            
            // Validation
            if (string.IsNullOrWhiteSpace(warehouse.Name))
                throw new ArgumentException("Tên kho không được để trống", nameof(warehouse.Name));
                
            if (string.IsNullOrWhiteSpace(warehouse.Code))
                throw new ArgumentException("Mã kho không được để trống", nameof(warehouse.Code));
                
            // Kiểm tra xem mã kho đã tồn tại chưa
            var existingWarehouse = await _warehouseRepository.GetByCodeAsync(warehouse.Code);
            if (existingWarehouse != null)
                throw new InvalidOperationException($"Mã kho '{warehouse.Code}' đã tồn tại");
                
            // Thiết lập ngày tạo
            warehouse.CreatedDate = DateTime.UtcNow;
            
            return await _warehouseRepository.AddAsync(warehouse);
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int id)
        {
            _logger.LogInformation("Lấy thông tin kho theo Id: {Id}", id);
            return await _warehouseRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            _logger.LogInformation("Lấy tất cả các kho");
            return await _warehouseRepository.GetAllAsync();
        }

        public async Task<Warehouse> UpdateWarehouseAsync(int id, Warehouse warehouse)
        {
            _logger.LogInformation("Cập nhật thông tin kho: {Id}, {Name}", id, warehouse.Name);
            
            // Validation
            if (string.IsNullOrWhiteSpace(warehouse.Name))
                throw new ArgumentException("Tên kho không được để trống", nameof(warehouse.Name));
                
            if (string.IsNullOrWhiteSpace(warehouse.Code))
                throw new ArgumentException("Mã kho không được để trống", nameof(warehouse.Code));
                
            // Kiểm tra xem kho có tồn tại không
            var existingWarehouse = await _warehouseRepository.GetByIdAsync(id);
            if (existingWarehouse == null)
                throw new NotFoundException($"Không tìm thấy kho với Id {id}");
                
            // Kiểm tra xem mã kho đã tồn tại chưa (nếu thay đổi mã)
            if (warehouse.Code != existingWarehouse.Code)
            {
                var warehouseWithSameCode = await _warehouseRepository.GetByCodeAsync(warehouse.Code);
                if (warehouseWithSameCode != null)
                    throw new InvalidOperationException($"Mã kho '{warehouse.Code}' đã tồn tại");
            }
            
            // Cập nhật thông tin
            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.Code = warehouse.Code;
            existingWarehouse.Address = warehouse.Address;
            existingWarehouse.City = warehouse.City;
            existingWarehouse.State = warehouse.State;
            existingWarehouse.Country = warehouse.Country;
            existingWarehouse.ZipCode = warehouse.ZipCode;
            existingWarehouse.ContactPerson = warehouse.ContactPerson;
            existingWarehouse.ContactEmail = warehouse.ContactEmail;
            existingWarehouse.ContactPhone = warehouse.ContactPhone;
            existingWarehouse.IsActive = warehouse.IsActive;
            existingWarehouse.LastModifiedDate = DateTime.UtcNow;
            
            await _warehouseRepository.UpdateAsync(existingWarehouse);
            return existingWarehouse;
        }

        public async Task<bool> DeleteWarehouseAsync(int id)
        {
            _logger.LogInformation("Xóa kho: {Id}", id);
            
            // Kiểm tra xem kho có tồn tại không
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                throw new NotFoundException($"Không tìm thấy kho với Id {id}");
                
            // Kiểm tra xem kho có hàng không
            var warehouseWithItems = await _warehouseRepository.GetWarehouseWithItemsAsync(id);
            if (warehouseWithItems != null && warehouseWithItems.WarehouseItems.Any())
                throw new InvalidOperationException($"Không thể xóa kho {id} vì vẫn còn hàng trong kho");
                
            await _warehouseRepository.DeleteAsync(warehouse);
            return true;
        }

        public async Task<WarehouseItem> AddStockAsync(int productId, int warehouseId, int quantity)
        {
            _logger.LogInformation("Thêm hàng vào kho: ProductId={ProductId}, WarehouseId={WarehouseId}, Quantity={Quantity}", 
                productId, warehouseId, quantity);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new WarehouseNotFoundException(warehouseId);
            }

            var warehouseItem = await _warehouseItemRepository.GetByWarehouseAndInventoryItemAsync(warehouseId, inventoryItem.Id);
            
            if (warehouseItem == null)
            {
                warehouseItem = new WarehouseItem
                {
                    InventoryItemId = inventoryItem.Id,
                    WarehouseId = warehouseId,
                    Warehouse = warehouse,
                    InventoryItem = inventoryItem,
                    Quantity = quantity,
                    Location = "Mặc định"
                };
                warehouseItem = await _warehouseItemRepository.AddAsync(warehouseItem);
            }
            else
            {
                warehouseItem.Quantity += quantity;
                await _warehouseItemRepository.UpdateAsync(warehouseItem);
            }

            // Cập nhật tổng số lượng trong InventoryItem
            await _inventoryItemRepository.UpdateQuantityAsync(inventoryItem.Id, quantity);

            // Ghi lại giao dịch
            var transaction = new InventoryTransaction
            {
                InventoryItemId = inventoryItem.Id,
                InventoryItem = inventoryItem,
                TransactionType = TransactionType.StockIn,
                Quantity = quantity,
                ReferenceNumber = Guid.NewGuid().ToString(),
                Notes = $"Thêm {quantity} sản phẩm vào kho {warehouse.Name}",
                CreatedBy = "System"
            };
            await _transactionRepository.AddAsync(transaction);

            return warehouseItem;
        }

        public async Task<WarehouseItem> RemoveStockAsync(int productId, int warehouseId, int quantity)
        {
            _logger.LogInformation("Lấy hàng ra khỏi kho: ProductId={ProductId}, WarehouseId={WarehouseId}, Quantity={Quantity}", 
                productId, warehouseId, quantity);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new WarehouseNotFoundException(warehouseId);
            }

            var warehouseItem = await _warehouseItemRepository.GetByWarehouseAndInventoryItemAsync(warehouseId, inventoryItem.Id);
            if (warehouseItem == null || warehouseItem.Quantity < quantity)
            {
                int availableQuantity = warehouseItem?.Quantity ?? 0;
                throw new InsufficientStockException(productId, quantity, availableQuantity);
            }

            warehouseItem.Quantity -= quantity;
            await _warehouseItemRepository.UpdateAsync(warehouseItem);

            // Cập nhật tổng số lượng trong InventoryItem
            await _inventoryItemRepository.UpdateQuantityAsync(inventoryItem.Id, -quantity);

            // Ghi lại giao dịch
            var transaction = new InventoryTransaction
            {
                InventoryItemId = inventoryItem.Id,
                InventoryItem = inventoryItem,
                TransactionType = TransactionType.StockOut,
                Quantity = quantity,
                ReferenceNumber = Guid.NewGuid().ToString(),
                Notes = $"Lấy {quantity} sản phẩm từ kho {warehouse.Name}",
                CreatedBy = "System"
            };
            await _transactionRepository.AddAsync(transaction);

            return warehouseItem;
        }

        public async Task<int> GetTotalStockAsync(int productId)
        {
            _logger.LogInformation("Lấy tổng số lượng tồn kho: ProductId={ProductId}", productId);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            var warehouseItems = await _warehouseItemRepository.GetByInventoryItemIdAsync(inventoryItem.Id);
            return warehouseItems.Sum(wi => wi.Quantity);
        }

        public async Task<IEnumerable<WarehouseItem>> GetStockByProductAsync(int productId)
        {
            _logger.LogInformation("Lấy thông tin tồn kho theo sản phẩm: ProductId={ProductId}", productId);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            return await _warehouseItemRepository.GetByInventoryItemIdAsync(inventoryItem.Id);
        }

        public async Task<IEnumerable<WarehouseItem>> GetStockByWarehouseAsync(int warehouseId)
        {
            _logger.LogInformation("Lấy thông tin tồn kho theo kho: WarehouseId={WarehouseId}", warehouseId);

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new WarehouseNotFoundException(warehouseId);
            }

            return await _warehouseItemRepository.GetByWarehouseIdAsync(warehouseId);
        }

        public async Task<bool> IsInStockAsync(int productId, int quantity)
        {
            _logger.LogInformation("Kiểm tra tồn kho: ProductId={ProductId}, Quantity={Quantity}", productId, quantity);

            try
            {
                var totalStock = await GetTotalStockAsync(productId);
                return totalStock >= quantity;
            }
            catch (InventoryItemNotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> ReserveStockAsync(int productId, int warehouseId, int quantity)
        {
            _logger.LogInformation("Đặt trước hàng: ProductId={ProductId}, WarehouseId={WarehouseId}, Quantity={Quantity}", 
                productId, warehouseId, quantity);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new WarehouseNotFoundException(warehouseId);
            }

            var warehouseItem = await _warehouseItemRepository.GetByWarehouseAndInventoryItemAsync(warehouseId, inventoryItem.Id);
            if (warehouseItem == null || warehouseItem.Quantity - warehouseItem.ReservedQuantity < quantity)
            {
                int availableQuantity = warehouseItem != null ? warehouseItem.Quantity - warehouseItem.ReservedQuantity : 0;
                throw new InsufficientStockException(productId, quantity, availableQuantity);
            }

            // Cập nhật số lượng đặt trước trong WarehouseItem
            await _warehouseItemRepository.ReserveStockAsync(warehouseItem.Id, quantity);

            // Cập nhật số lượng đặt trước trong InventoryItem
            await _inventoryItemRepository.ReserveStockAsync(inventoryItem.Id, quantity);

            // Ghi lại giao dịch
            var transaction = new InventoryTransaction
            {
                InventoryItemId = inventoryItem.Id,
                InventoryItem = inventoryItem,
                TransactionType = TransactionType.Reserve,
                Quantity = quantity,
                ReferenceNumber = Guid.NewGuid().ToString(),
                Notes = $"Đặt trước {quantity} sản phẩm từ kho {warehouse.Name}",
                CreatedBy = "System"
            };
            await _transactionRepository.AddAsync(transaction);

            return true;
        }

        public async Task<bool> ReleaseReservedStockAsync(int productId, int warehouseId, int quantity)
        {
            _logger.LogInformation("Giải phóng hàng đặt trước: ProductId={ProductId}, WarehouseId={WarehouseId}, Quantity={Quantity}", 
                productId, warehouseId, quantity);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new WarehouseNotFoundException(warehouseId);
            }

            var warehouseItem = await _warehouseItemRepository.GetByWarehouseAndInventoryItemAsync(warehouseId, inventoryItem.Id);
            if (warehouseItem == null)
            {
                throw new InvalidOperationException($"Không tìm thấy mặt hàng trong kho {warehouseId}");
            }

            int releaseQuantity = quantity;
            if (quantity > warehouseItem.ReservedQuantity)
            {
                releaseQuantity = warehouseItem.ReservedQuantity;
                _logger.LogWarning(
                    "Số lượng giải phóng ({Quantity}) lớn hơn số lượng đặt trước ({ReservedQuantity}). Chỉ giải phóng {ReleaseQuantity}.", 
                    quantity, warehouseItem.ReservedQuantity, releaseQuantity);
            }

            // Cập nhật số lượng đặt trước trong WarehouseItem
            await _warehouseItemRepository.ReleaseReservedStockAsync(warehouseItem.Id, releaseQuantity);

            // Cập nhật số lượng đặt trước trong InventoryItem
            await _inventoryItemRepository.ReleaseReservedStockAsync(inventoryItem.Id, releaseQuantity);

            // Ghi lại giao dịch
            var transaction = new InventoryTransaction
            {
                InventoryItemId = inventoryItem.Id,
                InventoryItem = inventoryItem,
                TransactionType = TransactionType.CancelReservation,
                Quantity = releaseQuantity,
                ReferenceNumber = Guid.NewGuid().ToString(),
                Notes = $"Giải phóng {releaseQuantity} sản phẩm đã đặt trước từ kho {warehouse.Name}",
                CreatedBy = "System"
            };
            await _transactionRepository.AddAsync(transaction);

            return true;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionHistoryAsync(int productId, DateTime? startDate, DateTime? endDate)
        {
            _logger.LogInformation("Lấy lịch sử giao dịch: ProductId={ProductId}, StartDate={StartDate}, EndDate={EndDate}", 
                productId, startDate, endDate);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            var transactions = await _transactionRepository.GetByInventoryItemIdAsync(inventoryItem.Id);
            
            if (startDate.HasValue && endDate.HasValue)
            {
                var filteredTransactions = await _transactionRepository.GetByDateRangeAsync(startDate.Value, endDate.Value);
                return filteredTransactions.Where(t => t.InventoryItemId == inventoryItem.Id);
            }
            
            return transactions;
        }

        public async Task<int> GetReservedStockAsync(int productId)
        {
            _logger.LogInformation("Lấy thông tin số lượng đặt trước: ProductId={ProductId}", productId);

            var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            return inventoryItem.ReservedQuantity;
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
        {
            _logger.LogInformation("Lấy danh sách sản phẩm có mức tồn kho thấp");
            
            var allItems = await _inventoryItemRepository.GetAllAsync();
            return allItems.Where(item => item.IsLowStock).ToList();
        }

        public async Task<InventoryItem> UpdateInventoryItemAsync(int productId, InventoryItem inventoryItem)
        {
            _logger.LogInformation("Cập nhật thông tin mục kho: ProductId={ProductId}", productId);

            var existingItem = await _inventoryItemRepository.GetByProductIdAsync(productId);
            if (existingItem == null)
            {
                throw new InventoryItemNotFoundException(productId);
            }

            // Cập nhật các trường có thể thay đổi
            existingItem.ProductName = inventoryItem.ProductName;
            existingItem.SKU = inventoryItem.SKU;
            existingItem.LowStockThreshold = inventoryItem.LowStockThreshold;

            var updatedItem = await _inventoryItemRepository.UpdateAsync(existingItem);
            return updatedItem;
        }
    }
} 