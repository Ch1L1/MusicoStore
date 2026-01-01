using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

public class OrderStatusLog : BaseEntity
{
    public int OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; }

    public int OrderStateId { get; set; }
    [ForeignKey(nameof(OrderStateId))]
    public OrderState OrderState { get; set; }

    public DateTime LogTime { get; set; }
}
