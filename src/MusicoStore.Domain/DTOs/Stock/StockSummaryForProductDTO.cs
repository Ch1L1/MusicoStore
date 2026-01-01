using MusicoStore.Domain.DTOs.Storage;

namespace MusicoStore.Domain.DTOs.Stock;

public class StockSummaryForProductDTO
{
    public int CurrentQuantity { get; set; }
    public StorageSummaryForStockDTO Storage { get; set; }
}
