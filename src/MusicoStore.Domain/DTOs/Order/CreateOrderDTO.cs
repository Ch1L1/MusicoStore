namespace MusicoStore.Domain.DTOs.Order;

public class CreateOrderDTO
{
    public int CustomerId { get; set; }
    public int? CustomerAddressId { get; set; }

    public IEnumerable<CreateOrderItemDTO> Items { get; set; } = new List<CreateOrderItemDTO>();
}
