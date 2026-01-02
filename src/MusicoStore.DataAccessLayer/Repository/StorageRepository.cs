using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;

namespace MusicoStore.DataAccessLayer.Repository;

public class StorageRepository(AppDbContext db) : GenericRepository<Storage>(db)
{
    public override async Task<IReadOnlyList<Storage>> GetAllAsync(CancellationToken ct)
        => await Db.Storages
            .Include(s => s.Address)
            .Include(s => s.Stocks)
                .ThenInclude(st => st.Product)
                    .ThenInclude(p => p.Manufacturer)
            .Include(s => s.Stocks)
                .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.CategoryAssignments)
                        .ThenInclude(a => a.ProductCategory)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<Storage?> GetByIdAsync(int id, CancellationToken ct)
        => Db.Storages
            .Include(s => s.Address)
            .Include(s => s.Stocks)
                .ThenInclude(st => st.Product)
                    .ThenInclude(p => p.Manufacturer)
            .Include(s => s.Stocks)
                .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.CategoryAssignments)
                        .ThenInclude(a => a.ProductCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);
}
