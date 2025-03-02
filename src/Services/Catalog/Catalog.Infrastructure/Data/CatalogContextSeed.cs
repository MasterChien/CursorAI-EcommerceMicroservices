using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Data;

public class CatalogContextSeed
{
    public static async Task SeedAsync(CatalogContext context, ILogger<CatalogContextSeed> logger)
    {
        try
        {
            if (!await context.Brands.AnyAsync())
            {
                await context.Brands.AddRangeAsync(GetPreconfiguredBrands());
                await context.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName} - Brands", nameof(CatalogContext));
            }

            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddRangeAsync(GetPreconfiguredCategories());
                await context.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName} - Categories", nameof(CatalogContext));
            }

            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(GetPreconfiguredProducts());
                await context.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName} - Products", nameof(CatalogContext));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static IEnumerable<Brand> GetPreconfiguredBrands()
    {
        return new List<Brand>
        {
            new Brand("Apple", "Công ty công nghệ hàng đầu thế giới"),
            new Brand("Samsung", "Tập đoàn điện tử đa quốc gia"),
            new Brand("Sony", "Công ty công nghệ và giải trí Nhật Bản"),
            new Brand("LG", "Tập đoàn điện tử Hàn Quốc"),
            new Brand("Dell", "Nhà sản xuất máy tính hàng đầu")
        };
    }

    private static IEnumerable<Category> GetPreconfiguredCategories()
    {
        return new List<Category>
        {
            new Category("Điện thoại", "Các loại điện thoại di động"),
            new Category("Laptop", "Máy tính xách tay"),
            new Category("Tablet", "Máy tính bảng"),
            new Category("TV", "Tivi thông minh"),
            new Category("Phụ kiện", "Các loại phụ kiện điện tử")
        };
    }

    private static IEnumerable<Product> GetPreconfiguredProducts()
    {
        return new List<Product>
        {
            new Product("iPhone 15 Pro", "iPhone mới nhất với chip A17 Pro", 27990000, 100, 1, 1),
            new Product("MacBook Pro M3", "Laptop mạnh mẽ với chip M3", 35990000, 50, 1, 2),
            new Product("iPad Pro", "Máy tính bảng chuyên nghiệp", 20990000, 75, 1, 3),
            new Product("Galaxy S24 Ultra", "Flagship Samsung với AI", 29990000, 80, 2, 1),
            new Product("Galaxy Book 4", "Laptop cao cấp của Samsung", 32990000, 40, 2, 2),
            new Product("Sony Bravia XR", "TV OLED cao cấp", 45990000, 30, 3, 4),
            new Product("LG OLED G4", "TV OLED đỉnh cao", 55990000, 25, 4, 4),
            new Product("Dell XPS 15", "Laptop doanh nhân cao cấp", 42990000, 35, 5, 2),
            new Product("Galaxy Buds 3", "Tai nghe không dây cao cấp", 3990000, 150, 2, 5),
            new Product("Apple AirPods Pro", "Tai nghe chống ồn cao cấp", 5990000, 120, 1, 5)
        };
    }
} 