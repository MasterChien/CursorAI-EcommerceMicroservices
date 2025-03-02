using System;
using System.Collections.Generic;

namespace Inventory.Application.DTOs
{
    public class InventoryCountDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public required string WarehouseName { get; set; }
        public required string CountNumber { get; set; }
        public DateTime CountDate { get; set; }
        public required string CountBy { get; set; }
        public required string Status { get; set; }
        public required string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ICollection<InventoryCountItemDto> CountItems { get; set; } = new List<InventoryCountItemDto>();
    }

    public class InventoryCountItemDto
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public required string ProductName { get; set; }
        public required string SKU { get; set; }
        public int ExpectedQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public int QuantityDifference { get; set; }
        public required string Notes { get; set; }
    }

    public class CreateInventoryCountDto
    {
        public int WarehouseId { get; set; }
        public required string CountBy { get; set; }
        public required string Notes { get; set; }
    }

    public class UpdateInventoryCountItemDto
    {
        public int InventoryItemId { get; set; }
        public int ActualQuantity { get; set; }
        public required string Notes { get; set; }
    }

    public class CompleteInventoryCountDto
    {
        public required string Notes { get; set; }
        public List<UpdateInventoryCountItemDto> CountItems { get; set; } = new List<UpdateInventoryCountItemDto>();
    }
} 