using MusicoStore.Domain.DTOs.Address;

namespace MusicoStore.Domain.DTOs.CustomerAddress;

public class CustomerAddressSummaryForCustomerDTO
{
    public bool IsMainAddress { get; set; }
    public AddressDTO Address { get; set; }
}
