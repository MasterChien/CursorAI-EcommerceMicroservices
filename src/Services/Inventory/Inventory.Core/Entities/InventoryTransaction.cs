using System;

namespace Inventory.Core.Entities
{
    public enum TransactionType
    {
        StockIn,
        StockOut,
        Adjustment,
        Reserve,
        CancelReservation,
        Reserved,    // Alias for Reserve
        Released     // Alias for CancelReservation
    }

    public class InventoryTransaction : BaseEntity
    {
        public int InventoryItemId { get; set; }
        public required InventoryItem InventoryItem { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public required string ReferenceNumber { get; set; }
        public required string Notes { get; set; }
        public required string CreatedBy { get; set; }
    }
} 