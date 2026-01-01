using MusicoStore.Domain.DTOs.Address;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IAddressService
{
    public Task<List<AddressDTO>> FindAllAsync(CancellationToken ct);
    public Task<AddressDTO> FindByIdAsync(int id, CancellationToken ct);
    public bool DoesExistById(int id);
    public Task<AddressDTO> CreateAsync(CreateAddressDTO dto, CancellationToken ct);
    public Task DeleteByIdAsync(int id, CancellationToken ct);
}
