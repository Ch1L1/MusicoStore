using MusicoStore.Domain.DTOs.Address;

namespace MusicoStore.Domain.DTOs.CustomerAddress;

public class UpsertCustomerAddressDTO
{
    public int? ExistingAddressId { get; set; }

    public CreateAddressDTO? NewAddress { get; set; }

    public bool IsMainAddress { get; set; }
}
