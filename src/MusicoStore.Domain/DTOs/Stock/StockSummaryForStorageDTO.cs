using MusicoStore.Domain.DTOs.Product;

namespace MusicoStore.Domain.DTOs.Stock;

public class StockSummaryForStorageDTO
{
    public ProductSummaryForStockDTO Product { get; set; }
    public int CurrentQuantity { get; set; }
}
