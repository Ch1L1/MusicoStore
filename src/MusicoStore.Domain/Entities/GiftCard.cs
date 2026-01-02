using System.ComponentModel.DataAnnotations.Schema;
using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.Domain.Entities;

public class GiftCard : BaseEntity
{
    /// <summary>
    /// Fixed discount amount (e.g. 200 CZK)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency of the gift card
    /// </summary>
    public Currency CurrencyCode { get; set; } = Currency.USD;

    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// Coupon codes generated for this gift card
    /// </summary>
    public ICollection<GiftCardCoupon> Coupons { get; set; }
        = new List<GiftCardCoupon>();
}
