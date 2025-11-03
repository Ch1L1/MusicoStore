using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer.Abstractions;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.DataAccessLayer.Models;

namespace MusicoStore.DataAccessLayer.Repository;

public class ProductRepository(AppDbContext db) : IRepository<Product>
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
        => await db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> FilterAsync(
        ProductFilterCriteria filter,
        CancellationToken ct)
    {
        IQueryable<Product> query = db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(p => p.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (!string.IsNullOrEmpty(filter.Description))
        {
            query = query.Where(p => p.Description.ToLower().Contains(filter.Description.ToLower()));
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.CurrentPrice <= filter.MaxPrice.Value);
        }

        if (!string.IsNullOrEmpty(filter.Category))
        {
            query = query.Where(p => p.ProductCategory!.Name.ToLower() == filter.Category.ToLower());
        }

        if (!string.IsNullOrEmpty(filter.Manufacturer))
        {
            query = query.Where(p => p.Manufacturer!.Name.ToLower() == filter.Manufacturer.ToLower());
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
