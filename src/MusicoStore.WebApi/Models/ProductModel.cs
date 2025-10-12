namespace MusicoStore.WebApi.Models;

public class ProductModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public string CurrencyCode { get; set; }
    public int ProductCategoryId { get; set; }
}
