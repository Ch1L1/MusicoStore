using MusicoStore.Domain.Entities;

public class GiftCardCoupon : BaseEntity
{
    public int GiftCardId { get; set; }
    public GiftCard GiftCard { get; set; } = default!;

    public string CouponCode { get; set; } = default!;

    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}
