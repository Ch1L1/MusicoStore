using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Records;

namespace MusicoStore.DataAccessLayer.Repository;

public class StockRepository(AppDbContext db) : GenericRepository<Stock>(db), IStockRepository
{
    public override async Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken ct)
        => await Db.Stocks
            .Include(s => s.Storage)
            .Include(s => s.Product)
                .ThenInclude(p => p.Manufacturer)
            .Include(s => s.Product)
                .ThenInclude(p => p.ProductCategory)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<Stock?> GetByIdAsync(int id, CancellationToken ct)
        => Db.Stocks
            .Include(s => s.Storage)
            .Include(s => s.Product)
                .ThenInclude(p => p.Manufacturer)
            .Include(s => s.Product)
                .ThenInclude(p => p.ProductCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Stock>> FilterAsync(StockFilterCriteria filter, CancellationToken ct)
    {
        IQueryable<Stock> query = Db.Stocks
            .Include(s => s.Storage)
            .Include(s => s.Product)
                .ThenInclude(p => p.Manufacturer)
            .Include(s => s.Product)
                .ThenInclude(p => p.ProductCategory)
            .AsQueryable();

        if (filter.storageId.HasValue)
        {
            query = query.Where(s => s.StorageId == filter.storageId);
        }

        if (filter.productId.HasValue)
        {
            query = query.Where(s => s.ProductId == filter.productId);
        }

        return await query.AsNoTracking().ToListAsync(ct);
    }
}
