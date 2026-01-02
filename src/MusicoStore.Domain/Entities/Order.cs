using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

public class Order : BaseEntity
{
    public int CustomerAddressId { get; set; }
    [ForeignKey(nameof(CustomerAddressId))]
    public CustomerAddress CustomerAddress { get; set; }

    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; }

    public GiftCardCoupon? GiftCardCoupon { get; set; }

    public ICollection<OrderedProduct> OrderedProducts { get; set; } = new List<OrderedProduct>();
    public ICollection<OrderStatusLog> StatusLog { get; set; } = new List<OrderStatusLog>();
}
