using System;

namespace Inventory.Core.Exceptions
{
    public class InventoryItemNotFoundException : InventoryException
    {
        public int? ProductId { get; }
        public string SKU { get; set; } = string.Empty;
        public int? InventoryItemId { get; }

        public InventoryItemNotFoundException(int inventoryItemId)
            : base($"Không tìm thấy mặt hàng trong kho với ID {inventoryItemId}")
        {
            InventoryItemId = inventoryItemId;
        }

        public InventoryItemNotFoundException(int? productId, string message)
            : base(message)
        {
            ProductId = productId;
        }

        public InventoryItemNotFoundException(string sku, string message)
            : base(message)
        {
            SKU = sku;
        }

        public InventoryItemNotFoundException(string message) : base(message) { }

        public InventoryItemNotFoundException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
} 