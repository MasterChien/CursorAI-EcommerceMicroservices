using Catalog.Core.Entities;

namespace Catalog.Core.Interfaces;

public interface IBrandRepository : IRepository<Brand>
{
    Task<Brand> GetBrandByNameAsync(string name);
    Task<IEnumerable<Brand>> GetBrandsWithProductsAsync();
    Task<Brand> GetBrandByIdWithProductsAsync(int id);
} 