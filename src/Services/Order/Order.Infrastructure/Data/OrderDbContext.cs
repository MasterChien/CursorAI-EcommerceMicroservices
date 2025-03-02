using Microsoft.EntityFrameworkCore;
using Order.Core.Entities;

namespace Order.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Core.Entities.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration for Order
            modelBuilder.Entity<Core.Entities.Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CustomerId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ShippingAddress).IsRequired().HasMaxLength(500);
                entity.Property(e => e.BillingAddress).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PaymentTransactionId).HasMaxLength(100);
                entity.Property(e => e.CancellationReason).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                // Tạo index để cải thiện hiệu suất truy vấn
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedDate);

                // Configure one-to-many relationship with StatusHistory
                entity.HasMany(o => o.StatusHistory)
                      .WithOne(h => h.Order)
                      .HasForeignKey(h => h.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration for OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ProductSku).HasMaxLength(50);
                entity.Property(e => e.PictureUrl).HasMaxLength(500);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");

                // Relationship with Order
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Tạo index để cải thiện hiệu suất truy vấn
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.ProductId);
            });

            // Configuration for OrderHistory
            modelBuilder.Entity<OrderHistory>(entity =>
            {
                entity.ToTable("OrderHistories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Comment).HasMaxLength(500);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                // Relationship with Order
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Tạo index để cải thiện hiệu suất truy vấn
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configuration for OrderStatusHistory
            modelBuilder.Entity<OrderStatusHistory>(entity => 
            {
                entity.ToTable("OrderStatusHistories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChangedDate).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.ChangedBy).HasMaxLength(100);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
} 