using MusicoStore.Domain.DTOs.Address;

namespace MusicoStore.Domain.DTOs.Storage;

public class StorageDTO
{
    public int StorageId { get; set; }
    public string Name { get; set; }
    public AddressDTO Address { get; set; }
    public int Capacity { get; set; }
    public string PhoneNumber { get; set; }
}
