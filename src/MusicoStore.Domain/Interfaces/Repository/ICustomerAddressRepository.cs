using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.Entities;

namespace MusicoStore.Domain.Interfaces.Repository;

public interface ICustomerAddressRepository : IRepository<CustomerAddress>
{
    Task<IReadOnlyList<CustomerAddress>> GetForCustomerAsync(int customerId, CancellationToken ct);

    Task<CustomerAddress?> GetMainAddressForCustomerAsync(int customerId, CancellationToken ct);
    Task<CustomerAddress?> GetByCustomerAndAddressAsync(int customerId, int addressId, CancellationToken ct);

    Task<Address?> FindDuplicateAddressAsync(CreateAddressDTO dto, CancellationToken ct);

    Task UnsetAllMainAddressesAsync(int customerId, CancellationToken ct);
}
