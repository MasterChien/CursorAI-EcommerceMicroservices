namespace Catalog.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public int BrandId { get; private set; }
    public Brand Brand { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; }

    public Product(string name, string description, decimal price, int stockQuantity, int brandId, int categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        BrandId = brandId;
        CategoryId = categoryId;
        CreatedDate = DateTime.UtcNow;
    }

    public void UpdateProduct(string name, string description, decimal price, int stockQuantity)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        LastModifiedDate = DateTime.UtcNow;
    }
} 