namespace MusicoStore.Domain.Entities;

public class Address : BaseEntity
{
    public string StreetName { get; set; }
    public string StreetNumber { get; set; }
    public string City { get; set; }
    public string PostalNumber { get; set; }
    public string CountryCode { get; set; }
}
