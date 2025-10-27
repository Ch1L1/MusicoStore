namespace MusicoStore.WebApi.Models;
using MusicoStore.DataAccessLayer.Enums;

public class ProductModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public Currency CurrencyCode { get; set; }
    public int ProductCategoryId { get; set; }
    public int ManufacturerId { get; set; }
}
