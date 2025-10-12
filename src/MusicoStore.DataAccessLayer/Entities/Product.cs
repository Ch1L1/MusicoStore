using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.DataAccessLayer.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal CurrentPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int ProductCategoryId { get; set; }

    [ForeignKey(nameof(ProductCategoryId))]
    public virtual ProductCategory? ProductCategory { get; set; }
    public int ManufacturerId { get; set; }

    [ForeignKey(nameof(ManufacturerId))]
    public virtual Manufacturer? Manufacturer { get; set; }
}
