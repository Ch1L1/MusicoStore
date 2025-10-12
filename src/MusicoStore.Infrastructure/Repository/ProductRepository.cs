using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Entities;

namespace MusicoStore.Infrastructure.Repository;

public class ProductRepository(AppDbContext db) : IRepository<Product>
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
        => await db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> FilterAsync(
        string? name,
        string? desc,
        decimal? priceMax,
        string? category,
        string? manufacturer,
        CancellationToken ct)
    {
        IQueryable<Product> query = db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
        }

        if (!string.IsNullOrEmpty(desc))
        {
            query = query.Where(p => p.Description.ToLower().Contains(desc.ToLower()));
        }

        if (priceMax != null)
        {
            query = query.Where(p => p.CurrentPrice <= priceMax);
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.ProductCategory!.Name.ToLower() == category.ToLower());
        }

        if (!string.IsNullOrEmpty(manufacturer))
        {
            query = query.Where(p => p.Manufacturer!.Name.ToLower() == manufacturer.ToLower());
        }

        return await query
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Product> AddAsync(Product product, CancellationToken ct)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        db.Products.Update(product);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        Product? entity = await db.Products.FindAsync([id], ct);

        if (entity != null)
        {
            db.Products.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
