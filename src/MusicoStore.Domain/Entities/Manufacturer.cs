using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

public class Manufacturer : BaseEntity
{
    public string Name { get; set; }
    public int AddressId { get; set; }

    [ForeignKey(nameof(AddressId))]
    public virtual Address? Address { get; set; }

    public IEnumerable<Product> Products { get; set; } = new List<Product>();
}
