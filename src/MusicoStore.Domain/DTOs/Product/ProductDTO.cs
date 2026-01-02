using MusicoStore.DataAccessLayer.Enums;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.DTOs.Stock;

namespace MusicoStore.Domain.DTOs.Product;

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public Currency CurrencyCode { get; set; }
    public List<ProductCategoryDTO> Categories { get; set; }
    public ManufacturerSummaryDTO? Manufacturer { get; set; }
    public List<StockSummaryForProductDTO> Stocks { get; set; }
    public string? ImagePath { get; set; }
    public int EditCount { get; set; }
    public int LastEditedByCustomerId { get; set; }
}
