using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Entities;

namespace MusicoStore.Infrastructure.Repository;

public class ProductRepository(AppDbContext db) : IRepository<Product>
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
        => await db.Products
            .Include(p => p.ProductCategory)
            .AsNoTracking()
            .ToListAsync(ct);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => db.Products
            .Include(p => p.ProductCategory)
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
