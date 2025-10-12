using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Entities;

namespace MusicoStore.Infrastructure.Repository;

public class ProductCategoryRepository(AppDbContext db) : IRepository<ProductCategory>
{
    public async Task<IReadOnlyList<ProductCategory>> GetAllAsync(CancellationToken ct)
        => await db.ProductCategories
            .Include(c => c.Products)
                .ThenInclude(p => p.Manufacturer)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<ProductCategory?> GetByIdAsync(int id, CancellationToken ct)
        => await db.ProductCategories
            .Include(c => c.Products)
                .ThenInclude(p => p.Manufacturer)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<ProductCategory> AddAsync(ProductCategory category, CancellationToken ct)
    {
        db.ProductCategories.Add(category);
        await db.SaveChangesAsync(ct);
        return category;
    }

    public async Task UpdateAsync(ProductCategory category, CancellationToken ct)
    {
        db.ProductCategories.Update(category);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        ProductCategory? entity = await db.ProductCategories.FindAsync([id], ct);
        if (entity != null)
        {
            db.ProductCategories.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
