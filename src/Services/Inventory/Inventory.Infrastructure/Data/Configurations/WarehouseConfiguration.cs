using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id).UseIdentityColumn();

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(w => w.Address)
                .HasMaxLength(200);

            builder.Property(w => w.City)
                .HasMaxLength(50);

            builder.Property(w => w.State)
                .HasMaxLength(50);

            builder.Property(w => w.Country)
                .HasMaxLength(50);

            builder.Property(w => w.ZipCode)
                .HasMaxLength(20);

            builder.Property(w => w.ContactPerson)
                .HasMaxLength(100);

            builder.Property(w => w.ContactEmail)
                .HasMaxLength(100);

            builder.Property(w => w.ContactPhone)
                .HasMaxLength(20);

            builder.Property(w => w.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasMany(w => w.WarehouseItems)
                .WithOne(wi => wi.Warehouse)
                .HasForeignKey(wi => wi.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(w => w.Code).IsUnique();
            builder.HasIndex(w => w.Name);
            builder.HasIndex(w => w.IsActive);
        }
    }
} 