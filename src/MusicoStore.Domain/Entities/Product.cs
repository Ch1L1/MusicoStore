namespace MusicoStore.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal CurrentPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int ProductCategoryId { get; set; }
}