namespace MusicoStore.Domain.DTOs.Address;

public class CreateAddressDTO
{
    public required string StreetName { get; set; }
    public required string StreetNumber { get; set; }
    public required string City { get; set; }
    public required string PostalNumber { get; set; }
    public required string CountryCode { get; set; }
}
