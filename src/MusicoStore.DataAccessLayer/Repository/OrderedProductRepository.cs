using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class OrderedProductRepository(AppDbContext db) : GenericRepository<OrderedProduct>(db)
{
    public override async Task<IReadOnlyList<OrderedProduct>> GetAllAsync(CancellationToken ct)
        => await Db.OrderedProducts
            .Include(op => op.Order)
            .Include(op => op.Product)
                .ThenInclude(p => p.ProductCategory)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<OrderedProduct?> GetByIdAsync(int id, CancellationToken ct)
        => Db.OrderedProducts
            .Include(op => op.Order)
            .Include(op => op.Product)
                .ThenInclude(p => p.ProductCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(op => op.Id == id, ct);
}
