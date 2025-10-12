namespace MusicoStore.DataAccessLayer.Entities;

public class ProductCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<Product>? Products { get; set; } = new List<Product>();
}
