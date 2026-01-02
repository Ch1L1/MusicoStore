using System.ComponentModel.DataAnnotations.Schema;
using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal CurrentPrice { get; set; }
    public Currency CurrencyCode { get; set; } = Currency.USD;
    public ICollection<ProductCategoryAssignment>? CategoryAssignments { get; set; }
    public int ManufacturerId { get; set; }

    [ForeignKey(nameof(ManufacturerId))]
    public virtual Manufacturer? Manufacturer { get; set; }

    public IEnumerable<Stock> Stocks { get; set; } = new List<Stock>();

    public ICollection<ProductEditLog> EditLogs { get; set; } = new List<ProductEditLog>();

    public string? ImagePath { get; set; } // relative path
}
