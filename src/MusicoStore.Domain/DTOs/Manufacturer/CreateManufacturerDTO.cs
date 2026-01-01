namespace MusicoStore.Domain.DTOs.Manufacturer;

public class CreateManufacturerDTO
{
    public required string Name { get; set; }
    public required int AddressId { get; set; }
}
