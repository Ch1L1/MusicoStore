using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.DTOs.ProductCategory;

namespace MusicoStore.Domain.DTOs;

public class SearchResultDTO
{
    public IEnumerable<ProductDTO> Products { get; set; } = Array.Empty<ProductDTO>();
    public IEnumerable<ProductCategoryDTO> Categories { get; set; } = Array.Empty<ProductCategoryDTO>();
    public IEnumerable<ManufacturerDTO> Manufacturers { get; set; } = Array.Empty<ManufacturerDTO>();
}
