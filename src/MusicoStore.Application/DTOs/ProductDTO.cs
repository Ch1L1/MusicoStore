using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicoStore.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal CurrentPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public ProductCategoryDto? ProductCategory { get; set; }
}
