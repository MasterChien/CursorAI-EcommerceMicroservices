namespace Catalog.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ICollection<Product> Products { get; private set; }

    public Category(string name, string description)
    {
        Name = name;
        Description = description;
        Products = new List<Product>();
        CreatedDate = DateTime.UtcNow;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        LastModifiedDate = DateTime.UtcNow;
    }
} 