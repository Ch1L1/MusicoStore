using System.Text.Json.Serialization;

namespace MusicoStore.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
