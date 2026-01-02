using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class ManufacturerRepository(AppDbContext db) : GenericRepository<Manufacturer>(db)
{
    public override async Task<IReadOnlyList<Manufacturer>> GetAllAsync(CancellationToken ct)
        => await Db.Manufacturers
            .Include(m => m.Address)
            .Include(m => m.Products)
                .ThenInclude(p => p.CategoryAssignments)
                    .ThenInclude(a => a.ProductCategory)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<Manufacturer?> GetByIdAsync(int id, CancellationToken ct)
        => Db.Manufacturers
            .Include(m => m.Address)
            .Include(m => m.Products)
                .ThenInclude(p => p.CategoryAssignments)
                    .ThenInclude(a => a.ProductCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);
}
