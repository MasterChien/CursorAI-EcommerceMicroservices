using System;
using System.Collections.Generic;

namespace Inventory.Application.DTOs
{
    public class StockTransferDto
    {
        public int Id { get; set; }
        public required string TransferNumber { get; set; }
        public int SourceWarehouseId { get; set; }
        public required string SourceWarehouseName { get; set; }
        public int DestinationWarehouseId { get; set; }
        public required string DestinationWarehouseName { get; set; }
        public DateTime TransferDate { get; set; }
        public required string Status { get; set; }
        public required string RequestedBy { get; set; }
        public required string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public required string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ICollection<StockTransferItemDto> TransferItems { get; set; } = new List<StockTransferItemDto>();
    }

    public class StockTransferItemDto
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public required string ProductName { get; set; }
        public required string SKU { get; set; }
        public int Quantity { get; set; }
        public required string Notes { get; set; }
    }

    public class CreateStockTransferDto
    {
        public int SourceWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public required string RequestedBy { get; set; }
        public required string Notes { get; set; }
        public List<CreateStockTransferItemDto> TransferItems { get; set; } = new List<CreateStockTransferItemDto>();
    }

    public class CreateStockTransferItemDto
    {
        public int InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public required string Notes { get; set; }
    }

    public class UpdateStockTransferStatusDto
    {
        public required string Status { get; set; }
        public required string ApprovedBy { get; set; }
        public required string Notes { get; set; }
    }
} 