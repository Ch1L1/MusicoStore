using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Repository;
using MusicoStore.Domain.Entities;


public class GiftCardRepository(AppDbContext db) : GenericRepository<GiftCard>(db)
{
    public override async Task<IReadOnlyList<GiftCard>> GetAllAsync(CancellationToken ct)
        => await Db.GiftCards
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<GiftCard?> GetByIdAsync(int id, CancellationToken ct)
        => Db.GiftCards
            .AsNoTracking()
            .FirstOrDefaultAsync(gc => gc.Id == id, ct);
}
