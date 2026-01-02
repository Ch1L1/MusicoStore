namespace MusicoStore.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<ProductCategoryAssignment>? CategoryAssignments { get; set; }

}
