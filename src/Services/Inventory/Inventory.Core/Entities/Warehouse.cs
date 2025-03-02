using System;
using System.Collections.Generic;

namespace Inventory.Core.Entities
{
    public class Warehouse : BaseEntity
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Country { get; set; }
        public required string ZipCode { get; set; }
        public required string ContactPerson { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
        public bool IsActive { get; set; }
        public ICollection<WarehouseItem> WarehouseItems { get; set; } = new List<WarehouseItem>();
    }
} 