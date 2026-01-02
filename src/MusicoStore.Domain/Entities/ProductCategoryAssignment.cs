using System.ComponentModel.DataAnnotations.Schema;

namespace MusicoStore.Domain.Entities;

/// <summary>
/// Junction entity for many-to-many relationship between Product and ProductCategory.
/// Uses a composite primary key (ProductId, ProductCategoryId).
/// </summary>
public class ProductCategoryAssignment
{
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = default!;

    public int ProductCategoryId { get; set; }

    [ForeignKey(nameof(ProductCategoryId))]
    public ProductCategory ProductCategory { get; set; } = default!;

    public bool IsPrimary { get; set; }
}
