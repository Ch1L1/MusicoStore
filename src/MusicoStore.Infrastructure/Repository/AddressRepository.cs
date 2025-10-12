using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Entities;

namespace MusicoStore.Infrastructure.Repository;

public class AddressRepository(AppDbContext db) : IRepository<Address>
{
    public async Task<IReadOnlyList<Address>> GetAllAsync(CancellationToken ct)
        => await db.Addresses
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<Address?> GetByIdAsync(int id, CancellationToken ct)
        => await db.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Address> AddAsync(Address address, CancellationToken ct)
    {
        db.Addresses.Add(address);
        await db.SaveChangesAsync(ct);
        return address;
    }

    public async Task UpdateAsync(Address address, CancellationToken ct)
    {
        db.Addresses.Update(address);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        Address? entity = await db.Addresses.FindAsync([id], ct);
        if (entity != null)
        {
            db.Addresses.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
