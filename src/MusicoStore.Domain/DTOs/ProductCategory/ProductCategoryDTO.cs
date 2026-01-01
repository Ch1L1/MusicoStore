using MusicoStore.Domain.DTOs.Product;

namespace MusicoStore.Domain.DTOs.ProductCategory;

public class ProductCategoryDTO
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public IEnumerable<ProductSummaryForCategoryDTO>? Products { get; set; }
}
