using System;

namespace Inventory.Application.DTOs
{
    public class WarehouseItemDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public required string WarehouseName { get; set; }
        public int InventoryItemId { get; set; }
        public required string ProductName { get; set; }
        public required string SKU { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public required string Location { get; set; }
        public DateTime? LastCountDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

    public class CreateWarehouseItemDto
    {
        public int WarehouseId { get; set; }
        public int InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public required string Location { get; set; }
    }

    public class UpdateWarehouseItemDto
    {
        public int Quantity { get; set; }
        public required string Location { get; set; }
    }
} 