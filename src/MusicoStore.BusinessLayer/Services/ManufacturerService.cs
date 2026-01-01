using AutoMapper;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class ManufacturerService(IRepository<Manufacturer> manufacturerRepository, IMapper mapper)
    : IManufacturerService
{
    public async Task<List<ManufacturerDTO>> FindAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Manufacturer> manufacturers = await manufacturerRepository.GetAllAsync(ct);
        return mapper.Map<List<ManufacturerDTO>>(manufacturers);
    }

    public async Task<ManufacturerDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        Manufacturer? manufacturer = await manufacturerRepository.GetByIdAsync(id, ct);
        return mapper.Map<ManufacturerDTO>(manufacturer);
    }

    public bool DoesExistById(int id)
        => manufacturerRepository.DoesEntityExist(id);

    public async Task<ManufacturerDTO> CreateAsync(CreateManufacturerDTO dto, CancellationToken ct)
    {
        Manufacturer manufacturer = mapper.Map<Manufacturer>(dto);
        Manufacturer created = await manufacturerRepository.AddAsync(manufacturer, ct);
        return mapper.Map<ManufacturerDTO>(created);
    }

    public async Task UpdateAsync(int id, UpdateManufacturerDTO dto, CancellationToken ct)
    {
        Manufacturer manufacturer = mapper.Map<Manufacturer>(dto);
        manufacturer.Id = id;
        await manufacturerRepository.UpdateAsync(manufacturer, ct);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
        => await manufacturerRepository.DeleteAsync(id, ct);
}
