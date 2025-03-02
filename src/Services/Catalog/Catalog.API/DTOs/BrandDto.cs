namespace Catalog.API.DTOs;

public class BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}

public class BrandWithProductsDto : BrandDto
{
    public ICollection<ProductDto> Products { get; set; }
}

public class CreateBrandDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class UpdateBrandDto
{
    public string Name { get; set; }
    public string Description { get; set; }
} 