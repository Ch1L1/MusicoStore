using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class ProductEditLogRepository(AppDbContext db) : GenericRepository<ProductEditLog>(db)
{
    public override async Task<IReadOnlyList<ProductEditLog>> GetAllAsync(CancellationToken ct)
        => await Db.ProductEditLogs
            .Include(l => l.Product)
            .Include(l => l.Customer)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<ProductEditLog?> GetByIdAsync(int id, CancellationToken ct)
        => Db.ProductEditLogs
            .Include(l => l.Product)
            .Include(l => l.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, ct);
}
