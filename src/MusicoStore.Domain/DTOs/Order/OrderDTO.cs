namespace MusicoStore.Domain.DTOs.Order;

public class OrderDTO
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int CustomerAddressId { get; set; }
    public DateTime CreatedAt { get; set; }

    public string CurrentState { get; set; } = null!;
    public decimal TotalAmount { get; set; }

    public IEnumerable<OrderItemDTO>? Items { get; set; }
}
