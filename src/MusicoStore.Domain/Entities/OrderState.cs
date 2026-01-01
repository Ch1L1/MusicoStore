namespace MusicoStore.Domain.Entities;

public class OrderState : BaseEntity
{
    public string Name { get; set; }

    public ICollection<OrderStatusLog> Logs { get; set; } = new List<OrderStatusLog>();
}
