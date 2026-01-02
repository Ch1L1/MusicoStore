namespace MusicoStore.Domain.DTOs.GiftCard;

public class GiftCardDTO
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public List<GiftCardCouponDTO> Coupons { get; set; }
}

