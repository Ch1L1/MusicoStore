namespace WebMVC.Models.GiftCard;

public class GiftCardCreateViewModel
{
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
