using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Data
{
    public class InventoryContextSeed
    {
        public static async Task SeedAsync(InventoryContext context, ILogger<InventoryContextSeed> logger)
        {
            try
            {
                if (!await context.Warehouses.AnyAsync())
                {
                    await context.Warehouses.AddRangeAsync(GetPreconfiguredWarehouses());
                    await context.SaveChangesAsync();
                    logger.LogInformation("Đã khởi tạo dữ liệu mẫu cho bảng Warehouses");
                }

                if (!await context.InventoryItems.AnyAsync())
                {
                    await context.InventoryItems.AddRangeAsync(GetPreconfiguredInventoryItems());
                    await context.SaveChangesAsync();
                    logger.LogInformation("Đã khởi tạo dữ liệu mẫu cho bảng InventoryItems");
                }

                if (!await context.WarehouseItems.AnyAsync())
                {
                    // Lấy danh sách kho và sản phẩm đã tạo
                    var warehouses = await context.Warehouses.ToListAsync();
                    var inventoryItems = await context.InventoryItems.ToListAsync();

                    if (warehouses.Any() && inventoryItems.Any())
                    {
                        await context.WarehouseItems.AddRangeAsync(GetPreconfiguredWarehouseItems(warehouses, inventoryItems));
                        await context.SaveChangesAsync();
                        logger.LogInformation("Đã khởi tạo dữ liệu mẫu cho bảng WarehouseItems");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi khởi tạo dữ liệu mẫu");
            }
        }

        private static IEnumerable<Warehouse> GetPreconfiguredWarehouses()
        {
            return new List<Warehouse>
            {
                new Warehouse
                {
                    Name = "Kho Hà Nội",
                    Code = "HN-WH",
                    Address = "Số 1 Đại Cồ Việt",
                    City = "Hà Nội",
                    State = "Hà Nội",
                    Country = "Việt Nam",
                    ZipCode = "100000",
                    ContactPerson = "Nguyễn Văn A",
                    ContactEmail = "nguyenvana@example.com",
                    ContactPhone = "0987654321",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Warehouse
                {
                    Name = "Kho Hồ Chí Minh",
                    Code = "HCM-WH",
                    Address = "Số 1 Võ Văn Ngân",
                    City = "Hồ Chí Minh",
                    State = "Hồ Chí Minh",
                    Country = "Việt Nam",
                    ZipCode = "700000",
                    ContactPerson = "Trần Thị B",
                    ContactEmail = "tranthib@example.com",
                    ContactPhone = "0123456789",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            };
        }

        private static IEnumerable<InventoryItem> GetPreconfiguredInventoryItems()
        {
            return new List<InventoryItem>
            {
                new InventoryItem
                {
                    ProductId = 1, // iPhone 15 Pro
                    ProductName = "iPhone 15 Pro",
                    SKU = "IP15PRO-128GB",
                    Quantity = 100,
                    ReservedQuantity = 0,
                    CreatedDate = DateTime.UtcNow
                },
                new InventoryItem
                {
                    ProductId = 2, // Galaxy S24 Ultra
                    ProductName = "Galaxy S24 Ultra",
                    SKU = "SS-S24U-256GB",
                    Quantity = 80,
                    ReservedQuantity = 0,
                    CreatedDate = DateTime.UtcNow
                },
                new InventoryItem
                {
                    ProductId = 3, // AirPods Pro
                    ProductName = "AirPods Pro",
                    SKU = "APP-2023",
                    Quantity = 150,
                    ReservedQuantity = 0,
                    CreatedDate = DateTime.UtcNow
                }
            };
        }

        private static IEnumerable<WarehouseItem> GetPreconfiguredWarehouseItems(List<Warehouse> warehouses, List<InventoryItem> inventoryItems)
        {
            var warehouseItems = new List<WarehouseItem>();

            // Kho Hà Nội
            var hanoi = warehouses.FirstOrDefault(w => w.Code == "HN-WH");
            if (hanoi != null)
            {
                // iPhone 15 Pro tại kho Hà Nội
                var iphone = inventoryItems.FirstOrDefault(i => i.ProductId == 1);
                if (iphone != null)
                {
                    warehouseItems.Add(new WarehouseItem
                    {
                        WarehouseId = hanoi.Id,
                        InventoryItemId = iphone.Id,
                        Warehouse = hanoi,
                        InventoryItem = iphone,
                        Quantity = 60,
                        ReservedQuantity = 0,
                        Location = "Khu A, Kệ 1",
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Samsung Galaxy S23 Ultra tại kho Hà Nội
                var samsung = inventoryItems.FirstOrDefault(i => i.ProductId == 2);
                if (samsung != null)
                {
                    warehouseItems.Add(new WarehouseItem
                    {
                        WarehouseId = hanoi.Id,
                        InventoryItemId = samsung.Id,
                        Warehouse = hanoi,
                        InventoryItem = samsung,
                        Quantity = 40,
                        ReservedQuantity = 0,
                        Location = "Khu A, Kệ 2",
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Xiaomi 13 Pro tại kho Hà Nội
                var xiaomi = inventoryItems.FirstOrDefault(i => i.ProductId == 3);
                if (xiaomi != null)
                {
                    warehouseItems.Add(new WarehouseItem
                    {
                        WarehouseId = hanoi.Id,
                        InventoryItemId = xiaomi.Id,
                        Warehouse = hanoi,
                        InventoryItem = xiaomi,
                        Quantity = 30,
                        ReservedQuantity = 0,
                        Location = "Khu A, Kệ 3",
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }

            // Kho Hồ Chí Minh
            var hcm = warehouses.FirstOrDefault(w => w.Code == "HCM-WH");
            if (hcm != null)
            {
                // iPhone 15 Pro tại kho Hồ Chí Minh
                var iphone = inventoryItems.FirstOrDefault(i => i.ProductId == 1);
                if (iphone != null)
                {
                    warehouseItems.Add(new WarehouseItem
                    {
                        WarehouseId = hcm.Id,
                        InventoryItemId = iphone.Id,
                        Warehouse = hcm,
                        InventoryItem = iphone,
                        Quantity = 50,
                        ReservedQuantity = 0,
                        Location = "Khu B, Kệ 1",
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Samsung Galaxy S23 Ultra tại kho Hồ Chí Minh
                var samsung = inventoryItems.FirstOrDefault(i => i.ProductId == 2);
                if (samsung != null)
                {
                    warehouseItems.Add(new WarehouseItem
                    {
                        WarehouseId = hcm.Id,
                        InventoryItemId = samsung.Id,
                        Warehouse = hcm,
                        InventoryItem = samsung,
                        Quantity = 35,
                        ReservedQuantity = 0,
                        Location = "Khu B, Kệ 2",
                        CreatedDate = DateTime.UtcNow
                    });
                }

                // Xiaomi 13 Pro tại kho Hồ Chí Minh
                var xiaomi = inventoryItems.FirstOrDefault(i => i.ProductId == 3);
                if (xiaomi != null)
                {
                    warehouseItems.Add(new WarehouseItem
                    {
                        WarehouseId = hcm.Id,
                        InventoryItemId = xiaomi.Id,
                        Warehouse = hcm,
                        InventoryItem = xiaomi,
                        Quantity = 25,
                        ReservedQuantity = 0,
                        Location = "Khu B, Kệ 3",
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }

            return warehouseItems;
        }
    }
} 