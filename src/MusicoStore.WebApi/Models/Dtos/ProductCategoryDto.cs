namespace MusicoStore.WebApi.Models.Dtos;

public class ProductCategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public IEnumerable<ProductSummaryForCategoryDto>? Products { get; set; }
}
