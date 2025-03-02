using System;
using System.Collections.Generic;

namespace Inventory.Core.Entities
{
    public class StockTransfer : BaseEntity
    {
        public required string TransferNumber { get; set; }
        public int SourceWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public DateTime TransferDate { get; set; }
        public StockTransferStatus Status { get; set; }
        public required string RequestedBy { get; set; }
        public required string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public required string Notes { get; set; }

        // Navigation properties
        public required Warehouse SourceWarehouse { get; set; }
        public required Warehouse DestinationWarehouse { get; set; }
        public ICollection<StockTransferItem> TransferItems { get; set; } = new List<StockTransferItem>();
    }

    public class StockTransferItem : BaseEntity
    {
        public int StockTransferId { get; set; }
        public int InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public required string Notes { get; set; }

        // Navigation properties
        public required StockTransfer StockTransfer { get; set; }
        public required InventoryItem InventoryItem { get; set; }
    }

    public enum StockTransferStatus
    {
        Draft = 0,
        Pending = 1,
        Approved = 2,
        InTransit = 3,
        Completed = 4,
        Cancelled = 5
    }
} 