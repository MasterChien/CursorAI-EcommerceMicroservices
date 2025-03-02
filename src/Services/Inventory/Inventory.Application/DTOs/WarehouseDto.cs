using System;
using System.Collections.Generic;

namespace Inventory.Application.DTOs
{
    public class WarehouseDto
    {
        public int Id { get; set; }
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
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

    public class CreateWarehouseDto
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
        public bool IsActive { get; set; } = true;
    }

    public class UpdateWarehouseDto
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Country { get; set; }
        public required string ZipCode { get; set; }
        public required string ContactPerson { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
        public bool IsActive { get; set; }
    }

    public class WarehouseWithItemsDto : WarehouseDto
    {
        public ICollection<WarehouseItemDto> WarehouseItems { get; set; } = new List<WarehouseItemDto>();
    }
} 