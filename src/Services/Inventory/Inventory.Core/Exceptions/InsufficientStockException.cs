using System;

namespace Inventory.Core.Exceptions
{
    public class InsufficientStockException : InventoryException
    {
        public int ProductId { get; }
        public int RequestedQuantity { get; }
        public int AvailableQuantity { get; }

        public InsufficientStockException(int productId, int requestedQuantity, int availableQuantity)
            : base($"Không đủ số lượng trong kho cho sản phẩm ID {productId}. Yêu cầu: {requestedQuantity}, Có sẵn: {availableQuantity}")
        {
            ProductId = productId;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }

        public InsufficientStockException(string message) : base(message) { }

        public InsufficientStockException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
} 