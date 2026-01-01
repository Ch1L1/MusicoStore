using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

public class ProductEditLog : BaseEntity
{
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }

    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; }

    public DateTime EditTime { get; set; }
}
