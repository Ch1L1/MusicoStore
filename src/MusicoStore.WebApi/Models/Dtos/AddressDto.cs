namespace MusicoStore.WebApi.Models.Dtos;

public class AddressDto
{
    public int AddressId { get; set; }
    public string StreetName { get; set; } = default!;
    public string StreetNumber { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PostalNumber { get; set; } = default!;
    public string CountryCode { get; set; } = default!;
}
