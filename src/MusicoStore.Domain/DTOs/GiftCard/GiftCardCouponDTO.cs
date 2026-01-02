namespace MusicoStore.Domain.DTOs.GiftCard;

public class GiftCardCouponDTO
{
    public int Id { get; set; }
    public string CouponCode { get; set; } = default!;
    public int? OrderId { get; set; }
}
