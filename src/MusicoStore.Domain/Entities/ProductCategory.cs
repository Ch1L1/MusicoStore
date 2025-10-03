namespace MusicoStore.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
