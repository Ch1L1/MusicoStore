using AutoMapper;
using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class AddressService(IRepository<Address> addressRepository, IMapper mapper) : IAddressService
{
    public async Task<List<AddressDTO>> FindAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Address> addresses = await addressRepository.GetAllAsync(ct);
        return mapper.Map<List<AddressDTO>>(addresses);
    }

    public async Task<AddressDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        Address? address = await addressRepository.GetByIdAsync(id, ct);
        return mapper.Map<AddressDTO>(address);
    }

    public bool DoesExistById(int id)
        => addressRepository.DoesEntityExist(id);

    public async Task<AddressDTO> CreateAsync(CreateAddressDTO dto, CancellationToken ct)
    {
        Address address = mapper.Map<Address>(dto);
        Address created = await addressRepository.AddAsync(address, ct);
        return mapper.Map<AddressDTO>(created);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
        => await addressRepository.DeleteAsync(id, ct);
}
