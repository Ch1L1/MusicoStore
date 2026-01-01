using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

public class CustomerAddress : BaseEntity
{
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; }

    public int AddressId { get; set; }
    [ForeignKey(nameof(AddressId))]
    public Address Address { get; set; }

    public bool IsMainAddress { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
