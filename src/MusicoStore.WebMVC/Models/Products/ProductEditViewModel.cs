using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebMVC.Models.Products;

public class ProductEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; }
    public int ProductCategoryId { get; set; } // Primary
    public int ManufacturerId { get; set; }
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Manufacturers { get; set; }
}
