using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models.Products;

public class ProductCreateViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; }
    public int ProductCategoryId { get; set; }
    public int ManufacturerId { get; set; }
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Manufacturers { get; set; }
}
