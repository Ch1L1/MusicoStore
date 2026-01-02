namespace MusicoStore.Domain.Interfaces.Repository;

public interface IGiftCardCouponRepository : IRepository<GiftCardCoupon>
{
    public Task<GiftCardCoupon?> GetByOrderIdAsync(int orderId, CancellationToken ct);
}
