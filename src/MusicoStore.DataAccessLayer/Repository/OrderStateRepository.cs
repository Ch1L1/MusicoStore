using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;

namespace MusicoStore.DataAccessLayer.Repository;

public class OrderStateRepository(AppDbContext db) : GenericRepository<OrderState>(db)
{
    public override Task<OrderState?> GetByIdAsync(int id, CancellationToken ct)
        => Db.OrderStates
            .Include(os => os.Logs)
            .AsNoTracking()
            .FirstOrDefaultAsync(os => os.Id == id, ct);
}
