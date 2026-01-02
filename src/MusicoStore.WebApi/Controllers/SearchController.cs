using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Controllers;

public class SearchController(
    IProductService productService,
    IProductCategoryService categoryService,
    IManufacturerService manufacturerService
) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query is required.");
        }

        string q = query.Trim().ToLowerInvariant();

        var productFilter = new ProductFilterRequestDTO { Name = query, Description = query };
        List<ProductDTO> products = await productService.FilterAsync(productFilter, ct);

        List<ProductCategoryDTO> categories = await categoryService.FindAllAsync(ct);
        categories = categories
            .Where(c => !string.IsNullOrEmpty(c.Name) && c.Name!.ToLowerInvariant().Contains(q))
            .ToList();

        List<ManufacturerDTO> manufacturers = await manufacturerService.FindAllAsync(ct);
        manufacturers = manufacturers
            .Where(m => !string.IsNullOrEmpty(m.Name) && m.Name!.ToLowerInvariant().Contains(q))
            .ToList();

        var result = new SearchResultDTO
        {
            Products = products,
            Categories = categories,
            Manufacturers = manufacturers
        };

        return Ok(result);
    }
}
