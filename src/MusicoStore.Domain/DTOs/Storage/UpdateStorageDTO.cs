namespace MusicoStore.Domain.DTOs.Storage;

public class UpdateStorageDTO
{
    public string Name { get; set; }
    public int AddressId { get; set; }
    public int Capacity { get; set; }
    public string PhoneNumber { get; set; }
}
