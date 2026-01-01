namespace MusicoStore.Domain.DTOs.Order;

public class OrderItemDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal PricePerItem { get; set; }
    public decimal LineTotal { get; set; }
}
