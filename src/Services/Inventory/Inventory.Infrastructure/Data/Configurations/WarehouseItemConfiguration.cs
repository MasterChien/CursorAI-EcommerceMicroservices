using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Configurations
{
    public class WarehouseItemConfiguration : IEntityTypeConfiguration<WarehouseItem>
    {
        public void Configure(EntityTypeBuilder<WarehouseItem> builder)
        {
            builder.ToTable("WarehouseItems");

            builder.HasKey(wi => wi.Id);
            builder.Property(wi => wi.Id).UseIdentityColumn();

            builder.Property(wi => wi.Quantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(wi => wi.ReservedQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(wi => wi.Location)
                .HasMaxLength(100);

            builder.Ignore(wi => wi.AvailableQuantity);

            builder.HasOne(wi => wi.Warehouse)
                .WithMany(w => w.WarehouseItems)
                .HasForeignKey(wi => wi.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wi => wi.InventoryItem)
                .WithMany()
                .HasForeignKey(wi => wi.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(wi => new { wi.WarehouseId, wi.InventoryItemId }).IsUnique();
        }
    }
} 