namespace MusicoStore.Domain.DTOs.Product;

public class ProductSummaryForCategoryDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public string CurrencyCode { get; set; }
    public string? ManufacturerName { get; set; }
}
