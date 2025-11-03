using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer.Abstractions;
using MusicoStore.DataAccessLayer.Entities;

namespace MusicoStore.DataAccessLayer.Repository;

public class ManufacturerRepository(AppDbContext db) : IRepository<Manufacturer>
{
    public async Task<IReadOnlyList<Manufacturer>> GetAllAsync(CancellationToken ct)
        => await db.Manufacturers
            .Include(m => m.Address)
            .Include(m => m.Products)
                .ThenInclude(p => p.ProductCategory)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<Manufacturer?> GetByIdAsync(int id, CancellationToken ct)
        => await db.Manufacturers
            .Include(m => m.Address)
            .Include(m => m.Products)
                .ThenInclude(p => p.ProductCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<Manufacturer> AddAsync(Manufacturer entity, CancellationToken ct)
    {
        db.Manufacturers.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Manufacturer entity, CancellationToken ct)
    {
        db.Manufacturers.Update(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        Manufacturer? manufacturer = await db.Manufacturers.FindAsync([id], ct);
        if (manufacturer != null)
        {
            db.Manufacturers.Remove(manufacturer);
            await db.SaveChangesAsync(ct);
        }
    }
}
