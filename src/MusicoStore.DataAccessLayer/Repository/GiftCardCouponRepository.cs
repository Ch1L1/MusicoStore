using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Repository;
using MusicoStore.Domain.Interfaces.Repository;

public class GiftCardCouponRepository(AppDbContext db) : GenericRepository<GiftCardCoupon>(db), IGiftCardCouponRepository
{
    public override async Task<IReadOnlyList<GiftCardCoupon>> GetAllAsync(CancellationToken ct)
        => await Db.GiftCardCoupons
            .Include(c => c.GiftCard)
            .Include(c => c.Order)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<GiftCardCoupon?> GetByIdAsync(int id, CancellationToken ct)
        => Db.GiftCardCoupons
            .Include(c => c.GiftCard)
            .Include(c => c.Order)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<GiftCardCoupon?> GetByOrderIdAsync(int orderId, CancellationToken ct)
        => Db.GiftCardCoupons
            .FirstOrDefaultAsync(c => c.OrderId == orderId, ct);
}
