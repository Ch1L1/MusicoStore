using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

public class OrderedProduct : BaseEntity
{
    public int OrderId { get; set; }
    [ForeignKey(nameof(OrderId))] public Order Order { get; set; }

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))] public Product Product { get; set; }

    public int Quantity { get; set; }
    public decimal PricePerItem { get; set; }
}
