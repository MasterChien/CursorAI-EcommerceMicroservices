using Catalog.Core.Entities;

namespace Catalog.Core.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category> GetCategoryByNameAsync(string name);
    Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
    Task<Category> GetCategoryByIdWithProductsAsync(int id);
} 