using MusicoStore.Domain.DTOs.Manufacturer;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IManufacturerService
{
    public Task<List<ManufacturerDTO>> FindAllAsync(CancellationToken ct);
    public Task<ManufacturerDTO> FindByIdAsync(int id, CancellationToken ct);
    public bool DoesExistById(int id);
    public Task<ManufacturerDTO> CreateAsync(CreateManufacturerDTO dto, CancellationToken ct);
    public Task UpdateAsync(int id, UpdateManufacturerDTO dto, CancellationToken ct);
    public Task DeleteByIdAsync(int id, CancellationToken ct);
}
