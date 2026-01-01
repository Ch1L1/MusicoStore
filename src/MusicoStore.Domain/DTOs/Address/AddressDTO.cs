namespace MusicoStore.Domain.DTOs.Address;

public class AddressDTO
{
    public int AddressId { get; set; }
    public string StreetName { get; set; }
    public string StreetNumber { get; set; }
    public string City { get; set; }
    public string PostalNumber { get; set; }
    public string CountryCode { get; set; }
}
