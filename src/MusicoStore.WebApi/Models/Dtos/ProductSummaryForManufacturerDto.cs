namespace MusicoStore.WebApi.Models.Dtos;

using MusicoStore.DataAccessLayer.Enums;

public class ProductSummaryForManufacturerDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal CurrentPrice { get; set; }
    public Currency CurrencyCode { get; set; }
    public string? CategoryName { get; set; }
}
