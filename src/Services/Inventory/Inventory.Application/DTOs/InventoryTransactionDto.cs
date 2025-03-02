using System;
using Inventory.Core.Entities;

namespace Inventory.Application.DTOs
{
    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public required string InventoryItemName { get; set; }
        public required string TransactionType { get; set; }
        public int Quantity { get; set; }
        public required string ReferenceNumber { get; set; }
        public required string Notes { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateInventoryTransactionDto
    {
        public int InventoryItemId { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public required string ReferenceNumber { get; set; }
        public required string Notes { get; set; }
        public required string CreatedBy { get; set; }
    }
} 