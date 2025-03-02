using System;
using System.Collections.Generic;

namespace Inventory.Application.DTOs
{
    public class InventoryItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public required string SKU { get; set; }
        public bool IsInStock { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock { get; set; }
        public DateTime? LastRestockDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

    public class CreateInventoryItemDto
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public required string SKU { get; set; }
        public int LowStockThreshold { get; set; } = 10; // Giá trị mặc định
    }

    public class UpdateInventoryItemDto
    {
        public required string ProductName { get; set; }
        public required string SKU { get; set; }
        public int? LowStockThreshold { get; set; }
    }

    public class InventoryItemWithTransactionsDto : InventoryItemDto
    {
        public ICollection<InventoryTransactionDto> Transactions { get; set; } = new List<InventoryTransactionDto>();
    }
} 