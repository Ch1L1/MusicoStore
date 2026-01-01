namespace MusicoStore.Domain.DTOs.Order;

public class ChangeOrderStateDTO
{
    public int OrderId { get; set; }
    public int NewStateId { get; set; }
}
