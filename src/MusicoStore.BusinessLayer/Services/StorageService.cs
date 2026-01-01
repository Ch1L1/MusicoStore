using AutoMapper;
using MusicoStore.Domain.DTOs.Stock;
using MusicoStore.Domain.DTOs.Storage;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Domain.Records;

namespace MusicoStore.BusinessLayer.Services;

public class StorageService(IRepository<Storage> storageRepository, IStockRepository stockRepository, IMapper mapper)
    : IStorageService
{
    public async Task<List<StorageDTO>> FindAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Storage> storages = await storageRepository.GetAllAsync(ct);
        return mapper.Map<List<StorageDTO>>(storages);
    }

    public async Task<StorageDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        Storage? storage = await storageRepository.GetByIdAsync(id, ct);
        return mapper.Map<StorageDTO>(storage);
    }

    public bool DoesExistById(int id) => storageRepository.DoesEntityExist(id);

    public async Task<StorageDTO> CreateAsync(CreateStorageDTO dto, CancellationToken ct)
    {
        Storage storage = mapper.Map<Storage>(dto);
        Storage created = await storageRepository.AddAsync(storage, ct);
        return mapper.Map<StorageDTO>(created);
    }

    public async Task UpdateAsync(int id, UpdateStorageDTO dto, CancellationToken ct)
    {
        Storage storage = mapper.Map<Storage>(dto);
        storage.Id = id;
        await storageRepository.UpdateAsync(storage, ct);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
        => await storageRepository.DeleteAsync(id, ct);

    public async Task<List<StockSummaryForStorageDTO>> FindStocksByStorageIdAsync(int storageId, CancellationToken ct)
    {
        IReadOnlyList<Stock> stocks = await stockRepository.FilterAsync(new StockFilterCriteria(storageId, null), ct);
        return mapper.Map<List<StockSummaryForStorageDTO>>(stocks);
    }

    public async Task<StockSummaryForStorageDTO> AddOrUpdateStockAsync(int storageId, StockUpdateDTO dto,
        CancellationToken ct)
    {
        var productId = dto.ProductId;
        var quantity = dto.QuantityDifference;

        IReadOnlyList<Stock> existingStocks =
            await stockRepository.FilterAsync(new StockFilterCriteria(storageId, productId), ct);
        Stock? existingStock = existingStocks
            .FirstOrDefault(s => s.StorageId == storageId && s.ProductId == dto.ProductId);

        if (existingStock != null) // Update
        {
            existingStock.CurrentQuantity += quantity;
            if (existingStock.CurrentQuantity < 0)
            {
                throw new InvalidOperationException(
                    $"Stock with id {existingStock.Id} does not have enough quantity {existingStock.CurrentQuantity}");
            }

            await stockRepository.UpdateAsync(existingStock, ct);
            return mapper.Map<StockSummaryForStorageDTO>(existingStock);
        }

        // Create new
        if (quantity < 0)
        {
            throw new InvalidOperationException("Cannot create a stock with quantity less than 0");
        }

        Stock newStock = mapper.Map<Stock>(dto);
        newStock.StorageId = storageId;
        Stock created = await stockRepository.AddAsync(newStock, ct);
        return mapper.Map<StockSummaryForStorageDTO>(created);
    }
}
