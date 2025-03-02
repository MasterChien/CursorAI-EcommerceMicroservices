using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).UseIdentityColumn();

            builder.Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.SKU)
                .HasMaxLength(50);

            builder.Property(i => i.Quantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(i => i.ReservedQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Ignore(i => i.AvailableQuantity);
            builder.Ignore(i => i.IsInStock);

            builder.HasIndex(i => i.ProductId).IsUnique();
            builder.HasIndex(i => i.SKU).IsUnique();
        }
    }
} 