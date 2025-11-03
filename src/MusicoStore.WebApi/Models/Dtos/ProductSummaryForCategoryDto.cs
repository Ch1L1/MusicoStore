namespace MusicoStore.WebApi.Models.Dtos;

using MusicoStore.DataAccessLayer.Enums;

public class ProductSummaryForCategoryDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal CurrentPrice { get; set; }
    public Currency CurrencyCode { get; set; }
    public string? ManufacturerName { get; set; }
}
