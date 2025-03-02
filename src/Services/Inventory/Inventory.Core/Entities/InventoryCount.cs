using System;
using System.Collections.Generic;

namespace Inventory.Core.Entities
{
    public class InventoryCount : BaseEntity
    {
        public int WarehouseId { get; set; }
        public required string CountNumber { get; set; }
        public DateTime CountDate { get; set; }
        public required string CountBy { get; set; }
        public InventoryCountStatus Status { get; set; }
        public required string Notes { get; set; }

        // Navigation properties
        public required Warehouse Warehouse { get; set; }
        public ICollection<InventoryCountItem> CountItems { get; set; } = new List<InventoryCountItem>();
    }

    public class InventoryCountItem : BaseEntity
    {
        public int InventoryCountId { get; set; }
        public int InventoryItemId { get; set; }
        public int ExpectedQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public int QuantityDifference => ActualQuantity - ExpectedQuantity;
        public required string Notes { get; set; }

        // Navigation properties
        public required InventoryCount InventoryCount { get; set; }
        public required InventoryItem InventoryItem { get; set; }
    }

    public enum InventoryCountStatus
    {
        Draft = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
} 