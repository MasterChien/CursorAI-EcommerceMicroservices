using Catalog.Core.Entities;

namespace Catalog.Core.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
    Task<Product> GetProductByIdWithBrandAndCategoryAsync(int id);
} 