using Microsoft.EntityFrameworkCore;
using MusicoStore.Application.Interfaces.IRepositories;
using MusicoStore.Domain.Entities;

namespace MusicoStore.Infrastructure.Persistence.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
        => await db.Products.Include(p => p.ProductCategory).AsNoTracking().ToListAsync(ct);

    public Task<Product?> GetAsync(int id, CancellationToken ct)
        => db.Products.Include(p => p.ProductCategory).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

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
        var p = await db.Products.FindAsync([id], ct);

        if (p is null)
        {
            return;
        }

        db.Products.Remove(p);
        await db.SaveChangesAsync(ct);
    }
}
