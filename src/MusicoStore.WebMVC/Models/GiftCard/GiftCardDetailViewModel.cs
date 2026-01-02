using MusicoStore.Domain.DTOs.GiftCard;

namespace WebMVC.Models.GiftCard;

public class GiftCardDetailViewModel
{
    public GiftCardDTO GiftCard { get; set; }
    public int TotalCoupons => GiftCard.Coupons.Count;
    public int UsedCoupons => GiftCard.Coupons.Count(c => c.OrderId.HasValue);
}
