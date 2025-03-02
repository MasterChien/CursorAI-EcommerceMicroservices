using System;

namespace Inventory.Core.Exceptions
{
    public class WarehouseNotFoundException : InventoryException
    {
        public int? WarehouseId { get; }
        public string WarehouseCode { get; set; } = string.Empty;

        public WarehouseNotFoundException(int warehouseId)
            : base($"Không tìm thấy kho hàng với ID {warehouseId}")
        {
            WarehouseId = warehouseId;
        }

        public WarehouseNotFoundException(string warehouseCode)
            : base($"Không tìm thấy kho hàng với mã {warehouseCode}")
        {
            WarehouseCode = warehouseCode;
        }

        public WarehouseNotFoundException(string message, int warehouseId) 
            : base(message)
        {
            WarehouseId = warehouseId;
        }

        public WarehouseNotFoundException(Exception innerException) 
            : base("Không tìm thấy kho hàng", innerException) { }

        public WarehouseNotFoundException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
} 