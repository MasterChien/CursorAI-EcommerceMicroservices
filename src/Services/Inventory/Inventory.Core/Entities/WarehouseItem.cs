using System;

namespace Inventory.Core.Entities
{
    public class WarehouseItem : BaseEntity
    {
        public int WarehouseId { get; set; }
        public required Warehouse Warehouse { get; set; }
        public int InventoryItemId { get; set; }
        public required InventoryItem InventoryItem { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity => Quantity - ReservedQuantity;
        public required string Location { get; set; } // Vị trí trong kho (kệ, ngăn, v.v.)
        public DateTime? LastCountDate { get; set; }
    }
} 