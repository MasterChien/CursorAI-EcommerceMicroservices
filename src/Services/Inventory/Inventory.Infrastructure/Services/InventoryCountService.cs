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
    public class InventoryCountService : IInventoryCountService
    {
        private readonly IInventoryCountRepository _inventoryCountRepository;
        private readonly IInventoryCountItemRepository _inventoryCountItemRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseItemRepository _warehouseItemRepository;
        private readonly IInventoryTransactionRepository _transactionRepository;
        private readonly ILogger<InventoryCountService> _logger;

        public InventoryCountService(
            IInventoryCountRepository inventoryCountRepository,
            IInventoryCountItemRepository inventoryCountItemRepository,
            IInventoryItemRepository inventoryItemRepository,
            IWarehouseRepository warehouseRepository,
            IWarehouseItemRepository warehouseItemRepository,
            IInventoryTransactionRepository transactionRepository,
            ILogger<InventoryCountService> logger)
        {
            _inventoryCountRepository = inventoryCountRepository;
            _inventoryCountItemRepository = inventoryCountItemRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseItemRepository = warehouseItemRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<InventoryCount> CreateInventoryCountAsync(int warehouseId, string countBy, string notes)
        {
            _logger.LogInformation("Tạo phiên kiểm kê mới: WarehouseId={WarehouseId}, CountBy={CountBy}", warehouseId, countBy);

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new WarehouseNotFoundException(warehouseId);
            }

            var inventoryCount = new InventoryCount
            {
                WarehouseId = warehouseId,
                Warehouse = warehouse,
                CountNumber = GenerateCountNumber(),
                CountDate = DateTime.UtcNow,
                Status = InventoryCountStatus.Draft,
                CountBy = countBy,
                Notes = notes
            };

            return await _inventoryCountRepository.AddAsync(inventoryCount);
        }

        public async Task<InventoryCount> GetInventoryCountByIdAsync(int id)
        {
            _logger.LogInformation("Lấy thông tin phiên kiểm kê: Id={Id}", id);
            var count = await _inventoryCountRepository.GetByIdAsync(id);
            if (count == null)
            {
                throw new NotFoundException($"Không tìm thấy phiên kiểm kê với Id {id}");
            }
            return count;
        }

        public async Task<IEnumerable<InventoryCount>> GetInventoryCountsByWarehouseAsync(int warehouseId)
        {
            _logger.LogInformation("Lấy danh sách phiên kiểm kê theo kho: WarehouseId={WarehouseId}", warehouseId);
            return await _inventoryCountRepository.GetByWarehouseIdAsync(warehouseId);
        }

        public async Task<IEnumerable<InventoryCount>> GetInventoryCountsByStatusAsync(InventoryCountStatus status)
        {
            _logger.LogInformation("Lấy danh sách phiên kiểm kê theo trạng thái: Status={Status}", status);
            return await _inventoryCountRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<InventoryCount>> GetAllInventoryCountsAsync()
        {
            _logger.LogInformation("Lấy tất cả các phiên kiểm kê");
            return await _inventoryCountRepository.GetAllAsync();
        }

        public async Task<InventoryCountItem> UpdateInventoryCountItemAsync(int countId, int inventoryItemId, int actualQuantity, string notes)
        {
            _logger.LogInformation("Cập nhật mục kiểm kê: CountId={CountId}, InventoryItemId={InventoryItemId}, ActualQuantity={ActualQuantity}", 
                countId, inventoryItemId, actualQuantity);

            var count = await _inventoryCountRepository.GetByIdAsync(countId);
            if (count == null)
            {
                throw new NotFoundException($"Không tìm thấy phiên kiểm kê với Id {countId}");
            }

            if (count.Status != InventoryCountStatus.InProgress)
            {
                throw new InvalidOperationException($"Không thể cập nhật mục kiểm kê khi phiên kiểm kê có trạng thái {count.Status}");
            }

            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(inventoryItemId);
            if (inventoryItem == null)
            {
                throw new InventoryItemNotFoundException(inventoryItemId);
            }

            var countItem = await _inventoryCountItemRepository.GetByInventoryCountAndItemAsync(countId, inventoryItemId);
            
            if (countItem == null)
            {
                // Lấy số lượng hiện tại từ kho
                var warehouseItem = await _warehouseItemRepository.GetByWarehouseAndInventoryItemAsync(count.WarehouseId, inventoryItemId);
                int currentQuantity = warehouseItem?.Quantity ?? 0;

                countItem = new InventoryCountItem
                {
                    InventoryCountId = countId,
                    InventoryItemId = inventoryItemId,
                    InventoryCount = count,
                    InventoryItem = await _inventoryItemRepository.GetByIdAsync(inventoryItemId),
                    ExpectedQuantity = currentQuantity,
                    ActualQuantity = actualQuantity,
                    Notes = notes
                };
                
                return await _inventoryCountItemRepository.AddAsync(countItem);
            }
            else
            {
                countItem.ActualQuantity = actualQuantity;
                countItem.Notes = notes;
                return await _inventoryCountItemRepository.UpdateAsync(countItem);
            }
        }

        public async Task<IEnumerable<InventoryCountItem>> GetInventoryCountItemsAsync(int countId)
        {
            _logger.LogInformation("Lấy danh sách mục kiểm kê: CountId={CountId}", countId);
            
            var count = await _inventoryCountRepository.GetByIdAsync(countId);
            if (count == null)
            {
                throw new NotFoundException($"Không tìm thấy phiên kiểm kê với Id {countId}");
            }
            
            return await _inventoryCountItemRepository.GetByInventoryCountIdAsync(countId);
        }

        public async Task<InventoryCount> StartInventoryCountAsync(int countId)
        {
            _logger.LogInformation("Bắt đầu phiên kiểm kê: CountId={CountId}", countId);
            
            var count = await _inventoryCountRepository.GetByIdAsync(countId);
            if (count == null)
            {
                throw new NotFoundException($"Không tìm thấy phiên kiểm kê với Id {countId}");
            }
            
            if (count.Status != InventoryCountStatus.Draft)
            {
                throw new InvalidOperationException($"Không thể bắt đầu phiên kiểm kê có trạng thái {count.Status}");
            }
            
            // Lấy tất cả các mặt hàng trong kho
            var warehouseItems = await _warehouseItemRepository.GetByWarehouseIdAsync(count.WarehouseId);
            
            // Tạo các mục kiểm kê cho từng mặt hàng
            foreach (var warehouseItem in warehouseItems)
            {
                var countItem = new InventoryCountItem
                {
                    InventoryCountId = countId,
                    InventoryItemId = warehouseItem.InventoryItemId,
                    InventoryCount = count,
                    InventoryItem = warehouseItem.InventoryItem,
                    ExpectedQuantity = warehouseItem.Quantity,
                    ActualQuantity = 0, // Sẽ được cập nhật khi kiểm kê
                    Notes = "Tự động tạo khi bắt đầu kiểm kê"
                };
                
                await _inventoryCountItemRepository.AddAsync(countItem);
            }
            
            // Cập nhật trạng thái phiên kiểm kê
            count.Status = InventoryCountStatus.InProgress;
            count.LastModifiedDate = DateTime.UtcNow;
            
            return await _inventoryCountRepository.UpdateAsync(count);
        }

        public async Task<InventoryCount> CompleteInventoryCountAsync(int countId, IEnumerable<InventoryCountItem> countItems, string notes)
        {
            _logger.LogInformation("Hoàn thành phiên kiểm kê: CountId={CountId}", countId);
            
            var count = await _inventoryCountRepository.GetByIdAsync(countId);
            if (count == null)
            {
                throw new NotFoundException($"Không tìm thấy phiên kiểm kê với Id {countId}");
            }
            
            if (count.Status != InventoryCountStatus.InProgress)
            {
                throw new InvalidOperationException($"Không thể hoàn thành phiên kiểm kê có trạng thái {count.Status}");
            }
            
            // Cập nhật số lượng thực tế cho các mục kiểm kê
            foreach (var item in countItems)
            {
                var existingItem = await _inventoryCountItemRepository.GetByInventoryCountAndItemAsync(countId, item.InventoryItemId);
                if (existingItem != null)
                {
                    existingItem.ActualQuantity = item.ActualQuantity;
                    existingItem.Notes = item.Notes;
                    await _inventoryCountItemRepository.UpdateAsync(existingItem);
                    
                    // Điều chỉnh số lượng trong kho nếu có sự khác biệt
                    int quantityDifference = existingItem.ActualQuantity - existingItem.ExpectedQuantity;
                    if (quantityDifference != 0)
                    {
                        var inventoryItem = await _inventoryItemRepository.GetByIdAsync(existingItem.InventoryItemId);
                        if (inventoryItem != null)
                        {
                            // Cập nhật số lượng trong WarehouseItem
                            var warehouseItem = await _warehouseItemRepository.GetByWarehouseAndInventoryItemAsync(count.WarehouseId, existingItem.InventoryItemId);
                            if (warehouseItem != null)
                            {
                                warehouseItem.Quantity = existingItem.ActualQuantity;
                                await _warehouseItemRepository.UpdateAsync(warehouseItem);
                            }
                            
                            // Cập nhật tổng số lượng trong InventoryItem
                            await _inventoryItemRepository.UpdateQuantityAsync(inventoryItem.Id, quantityDifference);
                            
                            // Ghi lại giao dịch
                            var transactionType = quantityDifference > 0 ? TransactionType.Adjustment : TransactionType.StockOut;
                            var transaction = new InventoryTransaction
                            {
                                InventoryItemId = inventoryItem.Id,
                                InventoryItem = inventoryItem,
                                TransactionType = transactionType,
                                Quantity = Math.Abs(quantityDifference),
                                ReferenceNumber = count.CountNumber,
                                Notes = $"Điều chỉnh từ kiểm kê #{count.CountNumber}: {quantityDifference}",
                                CreatedBy = count.CountBy
                            };
                            await _transactionRepository.AddAsync(transaction);
                        }
                    }
                }
            }
            
            // Cập nhật trạng thái phiên kiểm kê
            count.Status = InventoryCountStatus.Completed;
            count.Notes = notes;
            count.LastModifiedDate = DateTime.UtcNow;
            
            return await _inventoryCountRepository.UpdateAsync(count);
        }

        public async Task<InventoryCount> CancelInventoryCountAsync(int countId, string reason)
        {
            _logger.LogInformation("Hủy phiên kiểm kê: CountId={CountId}, Reason={Reason}", countId, reason);
            
            var count = await _inventoryCountRepository.GetByIdAsync(countId);
            if (count == null)
            {
                throw new NotFoundException($"Không tìm thấy phiên kiểm kê với Id {countId}");
            }
            
            if (count.Status == InventoryCountStatus.Completed)
            {
                throw new InvalidOperationException("Không thể hủy phiên kiểm kê đã hoàn thành");
            }
            
            // Cập nhật trạng thái phiên kiểm kê
            count.Status = InventoryCountStatus.Cancelled;
            count.Notes = $"Đã hủy: {reason}";
            count.LastModifiedDate = DateTime.UtcNow;
            
            return await _inventoryCountRepository.UpdateAsync(count);
        }

        public async Task<Dictionary<string, int>> GetInventoryDiscrepancySummaryAsync(int countId)
        {
            _logger.LogInformation("Lấy báo cáo chênh lệch kiểm kê: CountId={CountId}", countId);
            
            var count = await _inventoryCountRepository.GetByIdAsync(countId);
            if (count == null)
            {
                throw new NotFoundException($"Không tìm thấy phiên kiểm kê với Id {countId}");
            }
            
            var countItems = await _inventoryCountItemRepository.GetByInventoryCountIdAsync(countId);
            var summary = new Dictionary<string, int>
            {
                { "TotalItems", countItems.Count() },
                { "MatchingItems", countItems.Count(i => i.ExpectedQuantity == i.ActualQuantity) },
                { "DiscrepancyItems", countItems.Count(i => i.ExpectedQuantity != i.ActualQuantity) },
                { "OverageItems", countItems.Count(i => i.ActualQuantity > i.ExpectedQuantity) },
                { "ShortageItems", countItems.Count(i => i.ActualQuantity < i.ExpectedQuantity) },
                { "TotalDiscrepancy", countItems.Sum(i => i.ActualQuantity - i.ExpectedQuantity) }
            };
            
            return summary;
        }

        private string GenerateCountNumber()
        {
            return $"IC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
} 