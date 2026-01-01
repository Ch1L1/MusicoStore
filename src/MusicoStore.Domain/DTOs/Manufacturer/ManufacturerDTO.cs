using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.DTOs.Product;

namespace MusicoStore.Domain.DTOs.Manufacturer;

public class ManufacturerDTO
{
    public int ManufacturerId { get; set; }
    public string Name { get; set; }
    public AddressDTO? Address { get; set; }
    public IEnumerable<ProductSummaryForManufacturerDTO>? Products { get; set; }
}
