using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Configurations
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable("InventoryTransactions");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseIdentityColumn();

            builder.Property(t => t.TransactionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Quantity)
                .IsRequired();

            builder.Property(t => t.ReferenceNumber)
                .HasMaxLength(50);

            builder.Property(t => t.Notes)
                .HasMaxLength(500);

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(100);

            builder.HasOne(t => t.InventoryItem)
                .WithMany()
                .HasForeignKey(t => t.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.ReferenceNumber);
            builder.HasIndex(t => t.CreatedDate);
            builder.HasIndex(t => t.TransactionType);
        }
    }
}