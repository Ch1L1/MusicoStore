namespace MusicoStore.WebApi.Models.Dtos;

using MusicoStore.DataAccessLayer.Enums;

public class ProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal CurrentPrice { get; set; }
    public Currency CurrencyCode { get; set; }
    public CategorySummaryDto? Category { get; set; }
    public ManufacturerSummaryDto? Manufacturer { get; set; }
}

public class CategorySummaryDto
{
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class ManufacturerSummaryDto
{
    public int? ManufacturerId { get; set; }
    public string? ManufacturerName { get; set; }
}
