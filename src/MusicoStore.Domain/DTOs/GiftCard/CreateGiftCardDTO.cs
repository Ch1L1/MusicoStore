namespace MusicoStore.Domain.DTOs.GiftCard;

public class CreateGiftCardDTO
{
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
