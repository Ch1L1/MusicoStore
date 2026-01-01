namespace WebMVC.Models.Products;

public class ProductViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public string CurrencyCode { get; set; }
    public string CategoryName { get; set; }
    public string ManufacturerName { get; set; }
}
