using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class OrderStatusLogRepository(AppDbContext db) : GenericRepository<OrderStatusLog>(db)
{
    public override async Task<IReadOnlyList<OrderStatusLog>> GetAllAsync(CancellationToken ct)
        => await Db.OrderStatusLogs
            .Include(sl => sl.Order)
            .Include(sl => sl.OrderState)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<OrderStatusLog?> GetByIdAsync(int id, CancellationToken ct)
        => Db.OrderStatusLogs
            .Include(sl => sl.Order)
            .Include(sl => sl.OrderState)
            .AsNoTracking()
            .FirstOrDefaultAsync(sl => sl.Id == id, ct);
}
