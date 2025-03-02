using Catalog.Core.Entities;
using Catalog.Core.Interfaces;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(CatalogContext context) : base(context)
    {
    }

    public async Task<Category> GetCategoryByNameAsync(string name)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
    {
        return await _context.Categories
            .Include(c => c.Products)
                .ThenInclude(p => p.Brand)
            .ToListAsync();
    }

    public async Task<Category> GetCategoryByIdWithProductsAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Products)
                .ThenInclude(p => p.Brand)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
} 