using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;

namespace MusicoStore.DataAccessLayer.Repository;

public class OrderRepository(AppDbContext db) : GenericRepository<Order>(db)
{
    public override async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct)
        => await Db.Orders
            .Include(o => o.Customer)
            .Include(o => o.CustomerAddress)
                .ThenInclude(ca => ca.Address)
            .Include(o => o.OrderedProducts)
                .ThenInclude(op => op.Product)
            .Include(o => o.StatusLog)
                .ThenInclude(sl => sl.OrderState)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<Order?> GetByIdAsync(int id, CancellationToken ct)
        => Db.Orders
            .Include(o => o.Customer)
            .Include(o => o.CustomerAddress)
                .ThenInclude(ca => ca.Address)
            .Include(o => o.OrderedProducts)
                .ThenInclude(op => op.Product)
            .Include(o => o.StatusLog)
                .ThenInclude(sl => sl.OrderState)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);
}
