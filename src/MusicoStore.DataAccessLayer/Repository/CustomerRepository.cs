using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class CustomerRepository(AppDbContext db) : GenericRepository<Customer>(db)
{
    public override async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct)
        => await Db.Customers
            .Include(c => c.Addresses)
                .ThenInclude(ca => ca.Address)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<Customer?> GetByIdAsync(int id, CancellationToken ct)
        => Db.Customers
            .Include(c => c.Addresses)
                .ThenInclude(ca => ca.Address)
            .Include(c => c.Orders)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
}
