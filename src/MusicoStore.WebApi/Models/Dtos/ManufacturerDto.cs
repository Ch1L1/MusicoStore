namespace MusicoStore.WebApi.Models.Dtos;

public class ManufacturerDto
{
    public int ManufacturerId { get; set; }
    public string Name { get; set; } = default!;
    public AddressDto? Address { get; set; }
    public IEnumerable<ProductSummaryForManufacturerDto>? Products { get; set; }
}
