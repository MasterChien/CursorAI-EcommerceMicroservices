using System;

namespace Inventory.Core.Entities
{
    public class InventoryItem : BaseEntity
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity => Quantity - ReservedQuantity;
        public required string SKU { get; set; }
        public bool IsInStock => AvailableQuantity > 0;
        public DateTime? LastRestockDate { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock => AvailableQuantity <= LowStockThreshold;
    }
} 