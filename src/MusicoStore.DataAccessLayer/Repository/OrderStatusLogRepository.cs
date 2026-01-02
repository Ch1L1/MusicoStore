using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class OrderStatusLogRepository(AppDbContext db) : GenericRepository<OrderStatusLog>(db), IOrderStatusLogRepository
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

    public Task<string?> GetLatestStateNameAsync(int orderId, CancellationToken ct)
    => Db.OrderStatusLogs
        .Where(l => l.OrderId == orderId)
        .OrderByDescending(l => l.LogTime)
        .Select(l => l.OrderState!.Name)
        .AsNoTracking()
        .FirstOrDefaultAsync(ct);

}
