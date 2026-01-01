using MusicoStore.Domain.DTOs.Stock;
using MusicoStore.Domain.DTOs.Storage;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IStorageService
{
    public Task<List<StorageDTO>> FindAllAsync(CancellationToken ct);
    public Task<StorageDTO> FindByIdAsync(int id, CancellationToken ct);
    public bool DoesExistById(int id);
    public Task<StorageDTO> CreateAsync(CreateStorageDTO dto, CancellationToken ct);
    public Task UpdateAsync(int id, UpdateStorageDTO dto, CancellationToken ct);
    public Task DeleteByIdAsync(int id, CancellationToken ct);
    public Task<List<StockSummaryForStorageDTO>> FindStocksByStorageIdAsync(int storageId, CancellationToken ct);
    public Task<StockSummaryForStorageDTO> AddOrUpdateStockAsync(int storageId, StockUpdateDTO dto, CancellationToken ct);
}
