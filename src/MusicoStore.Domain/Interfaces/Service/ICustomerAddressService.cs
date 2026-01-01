using MusicoStore.Domain.DTOs.CustomerAddress;

namespace MusicoStore.Domain.Interfaces.Service;

public interface ICustomerAddressService
{
    Task<CustomerAddressSummaryForCustomerDTO> AddAddressAsync(
        int customerId,
        UpsertCustomerAddressDTO dto,
        CancellationToken ct);
}
