namespace MusicoStore.Domain.DTOs.Stock;

public class StockUpdateDTO
{
    public int ProductId { get; set; }

    public int QuantityDifference { get; set; }
}
