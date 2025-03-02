using Catalog.Core.Entities;
using Catalog.Core.Interfaces;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class BrandRepository : BaseRepository<Brand>, IBrandRepository
{
    public BrandRepository(CatalogContext context) : base(context)
    {
    }

    public async Task<Brand> GetBrandByNameAsync(string name)
    {
        return await _context.Brands
            .FirstOrDefaultAsync(b => b.Name == name);
    }

    public async Task<IEnumerable<Brand>> GetBrandsWithProductsAsync()
    {
        return await _context.Brands
            .Include(b => b.Products)
                .ThenInclude(p => p.Category)
            .ToListAsync();
    }

    public async Task<Brand> GetBrandByIdWithProductsAsync(int id)
    {
        return await _context.Brands
            .Include(b => b.Products)
                .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
} 