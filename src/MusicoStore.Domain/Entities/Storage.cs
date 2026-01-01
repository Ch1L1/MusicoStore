using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

public class Storage : BaseEntity
{
    public string Name { get; set; }
    public int AddressId { get; set; }

    [ForeignKey(nameof(AddressId))]
    public virtual Address Address { get; set; }

    public int Capacity { get; set; }
    public string PhoneNumber { get; set; }

    public IEnumerable<Stock> Stocks { get; set; } = new List<Stock>();
}
